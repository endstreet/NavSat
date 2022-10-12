using NavSat.Core.Abstrations.Models;

namespace NavSat.Core.Abstrations.Services
{
    public interface ISatelliteService
    {

        Satellite CreateFrom(int satelliteId);

    }
}
