using NavSat.Core.Abstrations.ApiClients;
using NavSat.Core.Abstrations.Models;
using NavSat.Core.ApiClients.Dtos;
using NavSat.Core.ApiClients.Mappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NavSat.Core.ApiClients {
    public class OrbitApiClient : IOrbitApiClient {
        private readonly IOrbitApiClientConfig _config;
        private readonly ISatOrbitMapper _mapper;

        public OrbitApiClient(IOrbitApiClientConfig config, ISatOrbitMapper mapper) {
            this._config = config;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<SatelliteOrbit>> GetOrbitsAsAtAsync(DateTimeOffset dateTimeOffset) {

            var utc = dateTimeOffset.UtcDateTime;

            using (var httpClient = new HttpClient()) {

                var uri = $"{_config.BaseUrl}/{utc.Year}/{utc.Month}/{utc.Day}";

                // TODO: Add Error Handling
                var json = await httpClient.GetStringAsync(uri);

                var far = JsonConvert.DeserializeObject<FullAlmanacResponse>(json);

                return far.Satellites.Select(s => _mapper.Map(s)).ToList();

            }



        }

    }
}
