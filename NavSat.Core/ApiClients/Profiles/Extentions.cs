using GeoJSON.Net.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NavSat.Core.ApiClients.Profiles
{
    internal static class Extentions
    {
        public static string WrapInFeatureCollection(this string features)
        {
            return @$"{{""type"": ""FeatureCollection"",""features"":[{features}]}}";
        }
        
    }
}
