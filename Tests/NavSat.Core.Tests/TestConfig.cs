using NavSat.Core.ApiClients;

namespace NavSat.Core.Tests
{
    internal class TestConfig : IOrbitApiClientConfig
    {
        public string BaseUrl => "https://www.gnssplanning.com/api/almanac";
    }
}
