using System;

namespace NavSat.Core.Abstrations.Models
{

    /// <summary>
    /// Represents a Satellite position in the sky as observed from an observer standing on Earth.
    /// </summary>
    public class SkyPlotCoordinate
    {

        /// <summary>
        /// The elevation above the horizon
        /// </summary>
        public double Elevation { get; set; }

        /// <summary>
        /// The Angle from True North (CW+)
        /// </summary>
        public double Azimuth { get; set; }

        public double[] ToElAzArray()
        {
            return new double[] { Elevation, Azimuth };
        }


        public static SkyPlotCoordinate From(params double[] elAz)
        {

            if (elAz.Length != 2)
            {
                throw new ArgumentException("2 Elements expected", "elAz");
            }

            return new SkyPlotCoordinate()
            {
                Elevation = elAz[0],
                Azimuth = elAz[1]
            };
        }

    }
}
