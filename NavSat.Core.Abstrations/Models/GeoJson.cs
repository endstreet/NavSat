using System;
using System.Collections.Generic;

namespace NavSat.Core.Abstrations.Models
{
    public class Properties
    {
        public string Name { get; set; }
        public string Prn { get; set; }
        public string Constellation { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; } //Point or LineString
        public List<List<double>> coordinates { get; set; }

    }

    public class Feature
    {
        public string type { get; set; }
        public Properties properties { get; set; }
        public Geometry geometry { get; set; }
    }

    public class FeatureCollection
    {
        public string type { get; set; }
        public List<Feature> features { get; set; }
    }

    //    A GeoJSON FeatureCollection:

    //   {
    //       "type": "FeatureCollection",
    //       "features": [{
    //           "type": "Feature",
    //           "geometry": {
    //               "type": "Point",
    //               "coordinates": [102.0, 0.5]
    //},
    //           "properties": {
    //    "prop0": "value0"
    //           }
    //       }, {
    //    "type": "Feature",
    //           "geometry": {
    //        "type": "LineString",
    //               "coordinates": [
    //                   [102.0, 0.0],
    //                   [103.0, 1.0],
    //                   [104.0, 0.0],
    //                   [105.0, 1.0]
    //               ]
    //           },
    //           "properties": {
    //               "prop0": "value0",
    //               "prop1": 0.0
    //           }
    //}
    //}]
    //   }
}
