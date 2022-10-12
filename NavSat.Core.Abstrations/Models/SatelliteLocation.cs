using System;

namespace NavSat.Core.Abstrations.Models
{
    public class SatelliteLocation
    {

        public SatelliteLocation() { }

        public SatelliteLocation(DateTimeOffset asAt, GeoCoordinate trace)
        {
            this.AsAt = asAt;
            this.Trace = trace;
        }

        public DateTimeOffset AsAt { get; set; }

        public GeoCoordinate Trace { get; set; }

    }
}
