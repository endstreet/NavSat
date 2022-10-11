using System;
using System.Linq;

namespace NavSat.Core.Abstrations.Models {

    /// <summary>
    /// Represents a Geographic Coordinate in Latitude (deg), Longitude (deg) and Height (masl)
    /// </summary>
    public class GeoCoordinate {

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Altitude { get; set; }

        public double[] ToLatLonHeightArray() {
            return new double[] { Latitude, Longitude, Altitude };
        }

        public static GeoCoordinate From(params double[] latLonHeight) {

            if (latLonHeight.Count() != 3) {
                throw new ArgumentException("Unexpected length", "latLonHeight");
            }

            return new GeoCoordinate() {
                Latitude = latLonHeight.ElementAt(0),
                Longitude = latLonHeight.ElementAt(1),
                Altitude = latLonHeight.ElementAt(2),
            };

        }


        public void CopyFrom(GeoCoordinate other) {
            this.Latitude = other.Latitude;
            this.Longitude = other.Longitude;
            this.Altitude = other.Altitude;
        }

    }
}
