using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;

namespace NavSat.Core.Services {
    public class SatelliteService : ISatelliteService {

        private readonly IConstellationService satSystemService;

        public SatelliteService(IConstellationService satSystemService) {
            this.satSystemService = satSystemService;
        }

        public Satellite CreateFrom(int satelliteId) {

            var system = satSystemService.For(satelliteId);
            if (system == null) {
                return null;
            }

            var prn = satelliteId - system.MinSatID + 1;

            return new Satellite(satelliteId, prn, system.Prefix);
        }



    }
}
