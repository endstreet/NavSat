using AutoMapper;
using GeoJSON.Net.Geometry;
using NavSat.Core.Abstrations.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Point = GeoJSON.Net.Geometry.Point;

namespace NavSat.Core.ApiClients.Profiles
{
    /// <summary>
    /// I don't think Automapper is particularly suited to map objects to geoJson
    /// The geojson.net and other libraries provide exellent features to accomplish this
    /// In the spirit of the Evaluation test I have created a custom converter for Automapper as use of Automapper was specified
    /// I used a combination of the geoJson.Net library objects newtonsoft and Stringbuilder to accomplish the task
    /// In this way we do not use my "naive" geoJson object..
    /// The API still only responds with objects that are actually used in the application
    /// (rather than a complete geoJson object padded with nulls)
    /// </summary>
    public class GeoJsonProfile : Profile
    {
        public GeoJsonProfile()
        {
            CreateMap<SatellitePath, string>().ConvertUsing<GeoJsonConverter>();
        }
    }
    public class GeoJsonConverter : ITypeConverter<SatellitePath, string>
    {

        public string Convert(SatellitePath source, string destination, ResolutionContext context)
        {
            return getGeometry(source);
        }
        /// <summary>
        /// Easy Extendable properties
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        string getProperties(SatellitePath src)
        {
            StringBuilder result = new StringBuilder();
            result.Append(@"""properties"":{");
            result.Append($@"""Name"":""{src.DisplayName}"",");
            result.Append($@"""IsHealthy"":{src.Orbit.IsHealthy.ToString().ToLower()},");
            result.Append($@"""Constellation"":""{src.Constellation}""");
            //Add more properties here...
            result.Append("},");
            return result.ToString();

        }
        string getGeometry(SatellitePath src)
        {
            StringBuilder result = new StringBuilder();
            //result.Append(@"""features"":[{""type"":""Feature"",");
            result.Append(@"{""type"":""Feature"",");
            result.Append(getProperties(src));
            result.Append(@"""geometry"":");

            if (src.Path.Count() == 1) //Point
            {
                result.Append(@getPoint(src.Path.First().Trace));
            }
            else //LineString
            {
                List<Position> coordinates = new List<Position>(src.Path.Select(x => new Position(x.Trace.Latitude, x.Trace.Longitude, x.Trace.Altitude)));
                string line = Newtonsoft.Json.JsonConvert.SerializeObject(new LineString(coordinates));
                result.Append(Newtonsoft.Json.JsonConvert.SerializeObject(new LineString(coordinates)));
            }
            result.Append("}");
            return result.ToString();
        }
        string getPoint(GeoCoordinate trace)
        {
            Point point = new Point(new Position(trace.Latitude, trace.Longitude, trace.Longitude));
            return Newtonsoft.Json.JsonConvert.SerializeObject(point);
        }
    }

}
