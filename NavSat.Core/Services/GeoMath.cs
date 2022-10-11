using NavSat.Core.Abstrations.Services;
using System;

namespace NavSat.Core.Services {
    public class GeoMath : IGeoMath {

        private static double WGS_A = 6378137.0;
        private static double WGS_B = 6356752.3142;
        private static double WGS_E2 = 0.00669437999013;
        private static double WGS_ES2 = 0.00673949674227;

        public double Rad2Deg(double rad) {
            return rad / Math.PI * 180.0;
        }

        public double Deg2Rad(double deg) {
            return deg / 180.0 * Math.PI;
        }

        public double[] Geo2Ecef(double lat, double lon, double hgt) {
            double temp;
            double N;

            double[] xyz = new double[3];

            temp = Math.Sin(lat);
            N = WGS_A / Math.Sqrt(1.0 - WGS_E2 * temp * temp);

            xyz[2] = (N * (1.0 - WGS_E2) + hgt) * temp;
            temp = Math.Cos(lat);
            xyz[0] = (N + hgt) * temp * Math.Cos(lon);
            xyz[1] = (N + hgt) * temp * Math.Sin(lon);

            return xyz;
        }

        public void Ecef2Geo(double[] xyz, out double lat, out double lon, out double hgt) {
            double p, temp, p1, p2, u;
            double theta, stheta, ctheta;

            p = Math.Sqrt(xyz[0] * xyz[0] + xyz[1] * xyz[1]);
            theta = Math.Atan2(xyz[2] * WGS_A, p * WGS_B);
            temp = Math.Sin(theta);
            stheta = temp * temp * temp;
            temp = Math.Cos(theta);
            ctheta = temp * temp * temp;

            lat = Math.Atan2(xyz[2] + WGS_ES2 * WGS_B * stheta, p - WGS_E2 * WGS_A * ctheta);

            lon = Math.Atan2(xyz[1], xyz[0]);
            u = Math.Atan(WGS_B * Math.Tan(lat) / WGS_A);
            p1 = p - WGS_A * Math.Cos(u);
            p2 = xyz[2] - WGS_B * Math.Sin(u);
            hgt = Math.Sqrt(p1 * p1 + p2 * p2);
            if (p1 < 0.0)
                hgt = -(hgt);
        }


        public double[] EcefToElAz(double[] stat, double[] xecf) {

            double elevation = 0;
            double azimuth = 0;


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
            if (d2 < 1e-6) { // we are near the center of earth, good GPS reception here ...
                d2 = 1e-6;
                slat = 0.0;
                clat = 1.0;
            }
            else {
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


            return new double[] { elevation, azimuth };

        }



        private double VectorSub(double[] xecf, double[] stat, out double[] rhoe) {
            if (xecf.Length != stat.Length)
                throw new ArgumentException();

            rhoe = new double[xecf.Length];
            double sum = 0;

            for (int i = 0; i < xecf.Length; ++i) {
                rhoe[i] = xecf[i] - stat[i];
                sum += rhoe[i] * rhoe[i];
            }

            return Math.Sqrt(sum);
        }
    }
}
