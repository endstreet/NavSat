using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;
using NavSat.Core.Utils;
using System;

namespace NavSat.Core.Services
{
    public class SatMath : ISatMath
    {

        private static readonly double rotrat = 7.293715E-05;
        private static double gm = 3.986005E+14;

        public EcefCoordinate CalculateEcef(DateTimeOffset dateTimeOffset, SatelliteOrbit orbit)
        {

            var time = new GPSTime(dateTimeOffset.UtcDateTime);

            long rtime = Convert.ToInt64(time.RogueTime);

            double cosi, scom, ccom, xpl, ypl, com, m0, dt, ec, radius;


            if (orbit.RootOfSemiMajorAxis <= 0.0 || orbit.SatId < 0)
            {
                return null;
            }

            // Difference of current time to almanach reference time in sec
            dt = rtime - (orbit.GpsWeek * 604800.0 + orbit.GpsSeconds);

            // first compute satellite position from almanac data
            m0 = orbit.M0 + dt / orbit.RootOfSemiMajorAxis * Math.Sqrt(gm / orbit.RootOfSemiMajorAxis);

            // Eccentric anomaly, solve Kepler's equation
            ec = EccAnom(orbit.Eccentricity, m0);

            // True Anomaly
            double v = Math.Atan2(Math.Sqrt(1 - orbit.Eccentricity * orbit.Eccentricity) * Math.Sin(ec), (Math.Cos(ec) - orbit.Eccentricity));

            radius = orbit.RootOfSemiMajorAxis * (1.0 - orbit.Eccentricity * Math.Cos(ec));   //  radius magnitude 

            //  Longitude of ascending node
            com = -rotrat * orbit.GpsSeconds + orbit.Omega0 + (orbit.OmegaDot - rotrat) * dt;

            //  orbital plane coordinates
            xpl = radius * Math.Cos(v + orbit.Omega);
            ypl = radius * Math.Sin(v + orbit.Omega);
            scom = Math.Sin(com);
            ccom = Math.Cos(com);

            //  inclination cosine
            cosi = Math.Cos(orbit.DeltaInclination);

            //  earth centered fixed coordinates
            return new EcefCoordinate()
            {
                X = xpl * ccom - ypl * cosi * scom,
                Y = xpl * scom + ypl * cosi * ccom,
                Z = ypl * Math.Sin(orbit.DeltaInclination)
            };
        }

        public virtual double EccAnom(double ec, double m)
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

    }
}
