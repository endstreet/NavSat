using System;

namespace NavSat.Core.Abstrations.Models
{
    public class ObservedSatelliteLocation : SatelliteLocation
    {

        public ObservedSatelliteLocation(DateTimeOffset asAt, GeoCoordinate trace, SkyPlotCoordinate relPos) : base(asAt, trace)
        {
            SkyCoordinate = relPos;
        }

        public SkyPlotCoordinate SkyCoordinate { get; set; }

    }
}
