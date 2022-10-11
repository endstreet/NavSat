using System;
using System.Linq;

namespace NavSat.Core.Abstrations.Models {

    /// <summary>
    /// Earth Centered, Earth Fixed Coordinate
    /// </summary>
    public class EcefCoordinate {

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public double[] ToXyzArray() {
            return new double[] { X, Y, Z };
        }

        public static EcefCoordinate From(params double[] xyz) {

            if (xyz.Count() != 3) {
                throw new ArgumentException("Unexpected length", "xyz");
            }

            return new EcefCoordinate() {
                X = xyz.ElementAt(0),
                Y = xyz.ElementAt(1),
                Z = xyz.ElementAt(2)
            };

        }

    }
}
