using AutoMapper;
using NavSat.Core.Abstrations.ApiClients;
using NavSat.Core.Abstrations.Models;
using NavSat.Core.ApiClients.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace NavSat.Core.ApiClients
{
    public class OrbitApiClient : IOrbitApiClient
    {
        private readonly IOrbitApiClientConfig _config;
        private readonly IMapper _mapper;

        public OrbitApiClient(IOrbitApiClientConfig config, IMapper mapper)
        {
            this._config = config;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<SatelliteOrbit>> GetOrbitsAsAtAsync(DateTimeOffset dateTimeOffset)
        {

            var utc = dateTimeOffset.UtcDateTime;

            using (var httpClient = new HttpClient())
            {

                var uri = $"{_config.BaseUrl}/{utc.Year}/{utc.Month}/{utc.Day}";

                string json = "";

                var res = await httpClient.GetAsync(uri);
                // I suspect the requirement hinted at a middleware request filter.
                // The EnsureSuccessStatusCode will throw the relevant exception anyways
                res.EnsureSuccessStatusCode();
                json = await res.Content.ReadAsStringAsync();

                var far = JsonConvert.DeserializeObject<FullAlmanacResponse>(json);
                //New Automapper map
                return far.Satellites.Select(s => _mapper.Map<SatelliteOrbit>(s)).ToList();

            }
        }
    }
}
