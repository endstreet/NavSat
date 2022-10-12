using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;
using NavSat.Core.Abstrations.Utils;
using NavSat.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NavSat.Core.Services
{
    public class AlmanacService : IAlmanacService
    {

        private static double rotrat = 7.293715E-05;
        private static double gm = 3.986005E+14;

        private readonly Dictionary<Satellite, SatelliteOrbit> _almanac = new Dictionary<Satellite, SatelliteOrbit>();
        private double[] _curtain = null;
        private readonly ISatelliteService _satService;
        private readonly IConstellationService _satSystemService;
        private readonly IGeoMath _geoMath;


        public AlmanacService(ISatelliteService satService, IConstellationService satSystemService, IGeoMath geoMath)
        {

            this._satService = satService;
            this._satSystemService = satSystemService;
            this._geoMath = geoMath;

            AlmanacTimeUTC = DateTime.MinValue;

        }

        public void DefineCurtain(double[] curtain)
        {
            if (curtain.Length > 0)
            {
                _curtain = new double[curtain.Length];
                curtain.CopyTo(_curtain, 0);
            }
        }

        public void Add(IEnumerable<SatelliteOrbit> satAlms)
        {
            foreach (SatelliteOrbit satAlm in satAlms)
            {
                var gpsTime = new GPSTime(satAlm.GpsWeek, satAlm.GpsSeconds);
                if (gpsTime.DateTimeUTC > AlmanacTimeUTC)
                    AlmanacTimeUTC = new DateTime(gpsTime.DateTimeUTC.Ticks, DateTimeKind.Utc);

                _almanac.Add(_satService.CreateFrom(satAlm.SatId), satAlm);
            }
        }

        public DateTime AlmanacTimeUTC
        {
            get;
            private set;
        }

        public bool GetSatellitePosition(IGPSTime time, Satellite sat, double[] pos, out double az, out double el, out double[] xecf)
        {
            long rtime = Convert.ToInt64(time.RogueTime);
            xecf = new double[3];

            if (!CalculateXYZ(time, sat, out xecf))
            {
                az = 0.0;
                el = 0.0;
                return false;
            }

            VectorToElAz(pos, xecf, out az, out el);
            return true;
        }

        public IEnumerable<Satellite> GetDops(IEnumerable<Constellation> systems, IGPSTime time, double elev_cutoff, double[] pos, int[] availbaleSatIds,
              out double gDOP, out double tDOP, out double pDOP, out double hDOP, out double vDOP)
        {

            // Set default return values.
            gDOP = tDOP = pDOP = hDOP = vDOP = double.NaN;

            double[,] Cov = new double[8, 8];
            for (int i = 0; i < 8; ++i)
                Cov[i, i] = 1e6;

            int ST_GPS = 0;
            int ST_GLO = 1;
            int ST_GAL = 2;
            int ST_COM = 3;
            int ST_QZS = 4;
            int ST_COUNT = 5;

            int[] numSats = new int[ST_COUNT];

            List<Satellite> usedSats = new List<Satellite>();
            IEnumerable<Satellite> visibleSats = GetVisibleSats(systems, time, elev_cutoff, pos);

            double[,] design = new double[visibleSats.Count(), 8];

            int n = 0;
            foreach (Satellite sat in visibleSats)
            {
                if (availbaleSatIds != null && !availbaleSatIds.Contains(sat.Id))
                    continue;

                double[] xecf;
                double az, el;
                if (!GetSatellitePosition(time, sat, pos, out az, out el, out xecf))
                    continue;

                double[] rhoe;
                double rho = VectorSub(xecf, pos, out rhoe);

                double temp = 1.0 / rho;
                design[n, 0] = rhoe[0] * temp;
                design[n, 1] = rhoe[1] * temp;
                design[n, 2] = rhoe[2] * temp;

                UpdateDesign(n, sat, design, numSats);
                UpdateCov(n, Cov, design);
                n++;

                usedSats.Add(sat);
            }

            // We need either 4 satellites in the same system or a combination
            // of 5 from both systems in order for the above matrix to make sense.

            // This needs to be verified in the case of Galileo and combined systems again //HL

            bool enoughSat = true;
            int sumSat = 0;
            for (int i = 0; i < ST_COUNT; ++i)
            {
                sumSat += numSats[i];
                if (numSats[i] < 4)
                    enoughSat = false;
            }

            if (!enoughSat && sumSat < 5)
                return new Satellite[0];

            // Set use[] to 1.0 if satellites are available for the specific constellation.

            double[] use = new double[ST_COUNT];
            for (int i = 0; i < ST_COUNT; ++i)
                use[i] = (numSats[i] > 0) ? 1.0 : 0.0;

            gDOP = Math.Sqrt(Cov[0, 0] + Cov[1, 1] + Cov[2, 2] +
                use[ST_GPS] * Cov[3, 3] +
                use[ST_GLO] * Cov[4, 4] +
                use[ST_GAL] * Cov[5, 5] +
                use[ST_COM] * Cov[6, 6] +
                use[ST_QZS] * Cov[7, 7]);

            tDOP = Math.Sqrt(
                use[ST_GPS] * Cov[3, 3] +
                use[ST_GLO] * Cov[4, 4] +
                use[ST_GAL] * Cov[5, 5] +
                use[ST_COM] * Cov[6, 6] +
                use[ST_QZS] * Cov[7, 7]);


            pDOP = Math.Sqrt(Cov[2, 2] + Cov[1, 1] + Cov[0, 0]);


            // Rotate 0..2
            double[,] Temp = new double[3, 3];

            double a, b, c;
            _geoMath.Ecef2Geo(pos, out a, out b, out c);

            double[,] T = CalcT(a, b);

            for (int k = 0; k < 3; k++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Temp[i, k] = 0.0;
                    for (int j = 0; j < 3; j++)
                        Temp[i, k] = Temp[i, k] + T[i, j] * Cov[j, k];
                }
            }

            double[,] Cplh = new double[3, 3];
            for (int k = 0; k < 3; k++)
            {
                for (int i = 0; i < 3; i++)
                {
                    Cplh[i, k] = 0.0;
                    for (int j = 0; j < 3; j++)
                        Cplh[i, k] = Cplh[i, k] + Temp[i, j] * T[k, j];
                }
            }

            hDOP = Math.Sqrt(Cplh[1, 1] + Cplh[0, 0]);
            vDOP = Math.Sqrt(Cplh[2, 2]);

            return usedSats;
        }

        public IEnumerable<Satellite> GetSats(IEnumerable<Constellation> systems, IGPSTime time)
        {
            var ret = new List<Satellite>();

            double[] pos = new double[] { 0, 0, 0 };
            double azSat, elSat;
            double[] satEcef;
            foreach (Satellite sat in GetSatellites(systems))
            {
                if (!GetSatellitePosition(time, sat, pos, out azSat, out elSat, out satEcef))
                    continue;

                ret.Add(sat);
            }

            ret.Sort();
            return ret;
        }

        public IEnumerable<Satellite> GetVisibleSats(IEnumerable<Constellation> systems, IGPSTime time, double cutoff, double[] pos)
        {
            var ret = new List<Satellite>();

            double azSat, elSat;
            double[] satEcef;
            foreach (Satellite sat in GetSatellites(systems))
            {
                //if ( !_Almanac[sat].IsHealthy )
                //    continue;

                if (!GetSatellitePosition(time, sat, pos, out azSat, out elSat, out satEcef))
                    continue;

                if (IsVisible(azSat, elSat, cutoff))
                    ret.Add(sat);
            }

            ret.Sort();
            return ret;
        }

        private IEnumerable<Satellite> GetSatellites(IEnumerable<Constellation> systems)
        {

            var allSystems = _satSystemService.All();

            if (systems.Count() == allSystems.Count())
                return _almanac.Keys.ToList();

            var q = from s in _almanac.Keys.AsQueryable()
                    where systems.Select(ss => ss.Prefix).Contains(s.Constellation)
                    select s;

            return q.ToList();
        }

        public SatelliteOrbit GetSatelliteById(int id)
        {
            return _almanac.Values.Where(sa => sa.SatId == id).SingleOrDefault();
        }

        public IEnumerable<SatelliteOrbit> FullAlmanac
        {
            get
            {
                return _almanac.Values.ToList();
            }
        }

        #region Helper Methods

        private bool CalculateXYZ(IGPSTime time, Satellite sat, out double[] xyz)
        {
            long rtime = Convert.ToInt64(time.RogueTime);

            double cosi, scom, ccom, xpl, ypl, com, m0, dt, ec, radius;

            xyz = new double[3];

            SatelliteOrbit ALM = _almanac[sat];

            if (ALM.RootOfSemiMajorAxis <= 0.0 || Math.Abs(ALM.SatId) != sat.Id)
            {
                // RK: We have problems with an almanac
                //     which provides all satellites regardless
                //     their visibility!
                xyz = new double[3];
                return false;
            }

            // Difference of current time to almanach reference time in sec
            dt = rtime - (ALM.GpsWeek * 604800.0 + ALM.GpsSeconds);

            // first compute satellite position from almanac data
            m0 = ALM.M0 + dt / ALM.RootOfSemiMajorAxis * Math.Sqrt(gm / ALM.RootOfSemiMajorAxis);

            // Eccentric anomaly, solve Kepler's equation
            ec = EccAnom(ALM.Eccentricity, m0);

            // True Anomaly
            double v = Math.Atan2(Math.Sqrt(1 - ALM.Eccentricity * ALM.Eccentricity) * Math.Sin(ec), (Math.Cos(ec) - ALM.Eccentricity));

            radius = ALM.RootOfSemiMajorAxis * (1.0 - ALM.Eccentricity * Math.Cos(ec));   //  radius magnitude 

            //  Longitude of ascending node
            com = -rotrat * ALM.GpsSeconds + ALM.Omega0 + (ALM.OmegaDot - rotrat) * dt;

            //  orbital plane coordinates
            xpl = radius * Math.Cos(v + ALM.Omega);
            ypl = radius * Math.Sin(v + ALM.Omega);
            scom = Math.Sin(com);
            ccom = Math.Cos(com);

            //  inclination cosine
            cosi = Math.Cos(ALM.DeltaInclination);

            //  earth centered fixed coordinates
            xyz[0] = xpl * ccom - ypl * cosi * scom;
            xyz[1] = xpl * scom + ypl * cosi * ccom;
            xyz[2] = ypl * Math.Sin(ALM.DeltaInclination);
            return true;
        }

        private double EccAnom(double ec, double m)
        {
            // arguments:
            // ec=eccentricity, m=mean anomaly,

            double M = 0;

            int maxIter = 30;
            int i = 0;

            double delta = 1E-12;

            //c      m=2.0*pi*(m-Math.floor(m)); 

            double E = M;
            double F = E - ec * Math.Sin(m) - m;

            while (Math.Abs(F) > delta && i < maxIter)
            {
                E = E - F / (1.0 - ec * Math.Cos(E));
                F = E - ec * Math.Sin(E) - m;
                i = i + 1;
            }
            return E;
        }

        private void UpdateCov(int n, double[,] cov, double[,] design)
        {
            double[] h = new double[8];
            double sum = 0.0;
            int i, j;

            for (i = 0; i < 8; i++)
            {
                h[i] = 0.0;
                for (j = 0; j < 8; j++)
                    h[i] += cov[i, j] * design[n, j];
                sum += design[n, i] * h[i];
            }
            sum = 1.0 / (sum + 1.0);

            for (i = 0; i < 8; i++)
                for (j = 0; j < 8; j++)
                    cov[i, j] -= sum * h[i] * h[j];
        }

        private void UpdateDesign(int n, Satellite sat, double[,] design, int[] numSats)
        {

            var system = _satSystemService.For(sat);

            if (system == _satSystemService.GPS())
            {
                numSats[0]++;
                design[n, 3] = 1.0;
                design[n, 4] = 0.0;
                design[n, 5] = 0.0; //HL 26.9.06
                design[n, 6] = 0.0;
                design[n, 7] = 0.0;
            }
            else if (system == _satSystemService.Glonass())
            {
                numSats[1]++;

                design[n, 3] = 0.0;
                design[n, 4] = 1.0;
                design[n, 5] = 0.0; //HL 26.9.06
                design[n, 6] = 0.0;
                design[n, 7] = 0.0;
            }
            else if (system == _satSystemService.Galileo())
            {
                numSats[2]++;
                design[n, 3] = 0.0;
                design[n, 4] = 0.0;
                design[n, 5] = 1.0; //HL 26.9.06
                design[n, 6] = 0.0;
                design[n, 7] = 0.0;
            }
            else if (system == _satSystemService.Compass())
            {
                numSats[3]++;
                design[n, 3] = 0.0;
                design[n, 4] = 0.0;
                design[n, 5] = 0.0;
                design[n, 6] = 1.0;
                design[n, 7] = 0.0;
            }
            else if (system == _satSystemService.QZSS())
            {
                numSats[4]++;
                design[n, 3] = 0.0;
                design[n, 4] = 0.0;
                design[n, 5] = 0.0;
                design[n, 6] = 0.0;
                design[n, 7] = 1.0;
            }
        }


        #region  TODO:  Move these to a math service??

        private double[,] CalcT(double lat, double lon)
        {
            double[,] T = new double[4, 4];

            T[0, 0] = -Math.Sin(lat) * Math.Cos(lon);
            T[0, 1] = -Math.Sin(lat) * Math.Sin(lon);
            T[0, 2] = Math.Cos(lat);
            T[1, 0] = -Math.Sin(lon);
            T[1, 1] = Math.Cos(lon);
            T[1, 2] = 0.0;
            T[2, 0] = Math.Cos(lat) * Math.Cos(lon);
            T[2, 1] = Math.Cos(lat) * Math.Sin(lon);
            T[2, 2] = Math.Sin(lat);

            return T;
        }

        private bool IsVisible(double az, double el, double cutoff)
        {
            if (el < cutoff)
                return false;
            return !InObstacle(az, el);
        }

        private bool InObstacle(double az, double el)
        {
            if (_curtain == null)
                return false;

            int idx = Convert.ToInt32(Math.Round(_geoMath.Rad2Deg(az)));
            if (_curtain[idx] > _geoMath.Rad2Deg(el))
                return true;

            return false;
        }

        private int VectorToElAz(double[] stat, double[] xecf, out double azimuth, out double elevation)
        {
            double[] rhoe = new double[3];
            double rhoe_norm;             // magnitude of rhoe vector
            double rho;

            // ----------------------------------------------------------------------
            // --- transform rhoe to the topograghic system and get rhot(meters). ---
            // ----------------------------------------------------------------------

            rhoe_norm = rho = VectorSub(xecf, stat, out rhoe);

            double stat_x = stat[0];
            double stat_y = stat[1];
            double stat_z = stat[2];
            double d1sq = stat_x * stat_x + stat_y * stat_y;
            double d1 = Math.Sqrt(d1sq);    // distance in x and y
            double slon, clon;
            //int    polar_case = 0;
            if (d1 < 1.0) // 1e-6 )
            {   // we are very near the north or south pole
                // shift the position by 1 m in x direction
                d1 = 1.0;
                d1sq = 1.0;
                stat_x = 1.0;
                stat_y = 0.0;
                //clon       = 1.0;
                //slon       = 0.0;
                //polar_case = 1;
            }
            //	else
            //	{
            clon = stat_x / d1;
            slon = stat_y / d1;
            //	}

            double[] rhodum = new double[3];
            rhodum[0] = clon * rhoe[0] + slon * rhoe[1];
            rhodum[1] = -slon * rhoe[0] + clon * rhoe[1];
            rhodum[2] = rhoe[2];

            double d2sq = d1sq + stat_z * stat_z;
            double d2 = Math.Sqrt(d2sq);        // distance in x, y and z
            double slat, clat;
            if (d2 < 1e-6)
            { // we are near the center of earth, good GPS reception here ...
                d2 = 1e-6;
                slat = 0.0;
                clat = 1.0;
            }
            else
            {
                slat = stat_z / d2;
                clat = d1 / d2;
            }

            double[] rhot = new double[3];
            rhot[0] = clat * rhodum[0] + slat * rhodum[2];
            rhot[1] = rhodum[1];
            rhot[2] = -slat * rhodum[0] + clat * rhodum[2];

            // --------------------------------------------------------------
            // now compute the elevation angle(degrees) of the satellite. but
            // first compute the angle from the zenith(degrees).
            // --------------------------------------------------------------

            double darg = rhot[0] / rho;
            if (darg < -1.0)
                darg = -1.0;
            else if (darg > 1.0)
                darg = 1.0;
            double zen = Math.Acos(darg);
            elevation = Math.PI / 2.0 - zen;

            // --------------------------------------------------------------
            // next compute azimuth in degrees; if we are near the poles az-
            // imuh will be measured eastward from grenwich rather than clockwise
            // from north.
            // --------------------------------------------------------------

            // --------------------------------------------------------------
            // first compute the projection of rhot onto the horizen plane and
            // call it rhohz(meters). also compute its magnitude.
            // --------------------------------------------------------------

            double[] rhohz = new double[3];
            rhohz[0] = 0.0;
            rhohz[1] = rhot[1];
            rhohz[2] = rhot[2];

            double rhohrz = Math.Sqrt(rhohz[1] * rhohz[1] + rhohz[2] * rhohz[2]);

            // skip down for the polar cases.

            //if (!polar_case) // fabs(slat) < 1.0)
            //{
            // --------------------------------------------------------------
            // the non-polar case follows.
            //
            // compute the angle rhohz makes with north(degrees). realize
            // that o.o.le.azimuth.le.180.0.
            // --------------------------------------------------------------
            azimuth = Math.Acos(rhohz[2] / rhohrz);
            // ---- remove the ambiguity(clockwise or counter-clockwise) from azimuth. ----
            double alpha = rhohz[1];
            if (alpha < 0.0)
                azimuth = 2.0 * Math.PI - azimuth;
            return 1;
        }

        private double VectorSub(double[] xecf, double[] stat, out double[] rhoe)
        {
            if (xecf.Length != stat.Length)
                throw new ArgumentException();

            rhoe = new double[xecf.Length];
            double sum = 0;

            for (int i = 0; i < xecf.Length; ++i)
            {
                rhoe[i] = xecf[i] - stat[i];
                sum += rhoe[i] * rhoe[i];
            }

            return Math.Sqrt(sum);
        }

        #endregion

        #endregion
    }
}
