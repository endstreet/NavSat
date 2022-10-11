using NavSat.Core.Abstrations.Models;
using NavSat.Core.ApiClients.Dtos;

namespace NavSat.Core.ApiClients.Mappers {
    public interface ISatOrbitMapper {
        SatelliteOrbit Map(SatAlmanac dto);
    }
}
