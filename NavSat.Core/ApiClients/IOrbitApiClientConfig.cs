namespace NavSat.Core.ApiClients {
    public interface IOrbitApiClientConfig {

        /// <summary>
        /// Points a base Url like https://www.gnssplanning.com/api/almanac
        /// </summary>
        string BaseUrl { get; }
    }
}
