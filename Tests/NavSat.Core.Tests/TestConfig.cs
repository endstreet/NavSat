using NavSat.Core.ApiClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavSat.Core.Tests
{
    internal class TestConfig : IOrbitApiClientConfig
    {
        public string BaseUrl => "https://www.gnssplanning.com/api/almanac";
    }
}
