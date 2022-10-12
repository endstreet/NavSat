using NavSat.Core.Abstrations.Models;
using System.Collections.Generic;

namespace NavSat.Core.Abstrations.Services
{
    public interface IConstellationService
    {

        Constellation GPS();
        Constellation Glonass();
        Constellation Galileo();
        Constellation Compass();
        Constellation QZSS();

        IEnumerable<Constellation> All();

        Constellation For(int satelliteId);

    }

    public static class ISatelliteSystemService_Extensions
    {
        public static Constellation For(this IConstellationService _this, Satellite satellite)
        {
            return _this.For(satellite.Id);
        }
    }
}
