using GeoJSON.Net.Geometry;
using NavSat.Core.Abstrations.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NavSat.Core.Services
{
    public static class GeoJson
    {
        public static string geoJsonConverter(SatellitePath src)
        {
            return getGeometry(src);
        }
        private static string getProperties(SatellitePath src)
        {
            StringBuilder result = new StringBuilder();
            result.Append("\"properties\":{");
            result.Append($"\"Name\":\"{src.DisplayName}\",");
            result.Append($"\"IsHealthy\":\"{src.Orbit.IsHealthy}\",");
            result.Append($"\"Constellation\":\"{src.Constellation}\"");
            //Add more properties here...
            result.Append("},");
            return result.ToString();
        }
        private static string getGeometry(SatellitePath src)
        {
            StringBuilder result = new StringBuilder();
            result.Append("{\"type\": \"FeatureCollection\" , \"features\":");
            result.Append("[{\"type\":\"Feature\",");
            result.Append(getProperties(src));
            result.Append("\"geometry\":");
            if (src.Path.Count() == 1) //Point
            {
                result.Append(getPoint(src.Path.First().Trace));
            }
            else //LineString
            {

                List<Position> coordinates = new List<Position>(src.Path.Select(x => new Position(x.Trace.Latitude, x.Trace.Longitude, x.Trace.Altitude)));
                string line = Newtonsoft.Json.JsonConvert.SerializeObject(new LineString(coordinates));
                result.Append(Newtonsoft.Json.JsonConvert.SerializeObject(new LineString(coordinates)));
            }
            result.Append("}]}");
            return result.ToString();
        }
        private static string getPoint(GeoCoordinate trace)
        {
            Point point = new Point(new Position(trace.Latitude, trace.Longitude, trace.Longitude));
            //"{\"coordinates\":[-2.124156,51.899523],\"type\":\"Point\"}"
            return Newtonsoft.Json.JsonConvert.SerializeObject(point);
        }

    }
}
