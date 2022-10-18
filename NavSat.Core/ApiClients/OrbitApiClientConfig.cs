namespace NavSat.Core.ApiClients
{
    public class OrbitApiClientConfig : IOrbitApiClientConfig
    {

        public OrbitApiClientConfig(string baseUrl)
        {
            BaseUrl = baseUrl;
        }
        public string BaseUrl { get; }
    }
}
