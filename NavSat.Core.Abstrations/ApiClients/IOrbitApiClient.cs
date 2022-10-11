using NavSat.Core.Abstrations.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NavSat.Core.Abstrations.ApiClients {
    public interface IOrbitApiClient {
        Task<IEnumerable<SatelliteOrbit>> GetOrbitsAsAtAsync(DateTimeOffset dateTimeOffset);
    }
}
