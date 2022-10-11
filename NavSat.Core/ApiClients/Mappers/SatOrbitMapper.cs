using NavSat.Core.Abstrations.Models;
using NavSat.Core.ApiClients.Dtos;

namespace NavSat.Core.ApiClients.Mappers {
    public class SatOrbitMapper : ISatOrbitMapper {

        private readonly double rhoalt = 57.295779513082;

        public SatelliteOrbit Map(SatAlmanac sd) {

            return new SatelliteOrbit() {
                A0 = sd.SatClockOffset,
                A1 = sd.SatClockDrift,
                DeltaInclination = (54 + sd.Inclination) / rhoalt,
                Eccentricity = sd.Eccentricity,
                GpsSeconds = sd.TimeOfAlmanac,
                GpsWeek = sd.GpsWeek,
                HealthCode = sd.Health,
                M0 = sd.MeanAnomaly / rhoalt,
                Omega = sd.Perigee / rhoalt,
                Omega0 = sd.RightAscension / rhoalt,
                OmegaDot = sd.AscensionRate / rhoalt / 1000.0,
                RootOfSemiMajorAxis = sd.SemiMajorAxis * sd.SemiMajorAxis,
                SatId = sd.SVN
            };

        }
    }
}
