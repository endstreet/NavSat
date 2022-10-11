using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;
using System.Collections.Generic;

namespace NavSat.Core.Services {
    public class ConstellationService : IConstellationService {

        private readonly Constellation _gps = new Constellation("GPS", 0, 'G', 1, 37);
        private readonly Constellation _glonass = new Constellation("Glonass", 1, 'R', 38, 68);
        private readonly Constellation _galileo = new Constellation("Galileo", 2, 'E', 201, 263);
        private readonly Constellation _compass = new Constellation("Compass", 3, 'C', 264, 293); //BeiDou
        private readonly Constellation _qzss = new Constellation("QZSS", 4, 'J', 111, 118);

        public IEnumerable<Constellation> All() {
            var list = new List<Constellation> {
                Compass(),
                Galileo(),
                Glonass(),
                GPS(),
                QZSS()
            };

            list.Sort();

            return list;
        }

        public Constellation Compass() {
            return _compass;
        }

        public Constellation Galileo() {
            return _galileo;
        }

        public Constellation Glonass() {
            return _glonass;
        }

        public Constellation GPS() {
            return _gps;
        }

        public Constellation QZSS() {
            return _qzss;
        }

        public Constellation For(int satelliteId) {

            if (satelliteId >= _gps.MinSatID && satelliteId <= _gps.MaxSatID)
                return _gps;
            else if (satelliteId >= _glonass.MinSatID && satelliteId <= _glonass.MinSatID)
                return _glonass;
            else if (satelliteId >= _qzss.MinSatID && satelliteId <= _qzss.MaxSatID)
                return _qzss;
            else if (satelliteId >= _galileo.MinSatID && satelliteId <= _galileo.MaxSatID)
                return _galileo;
            else if (satelliteId >= _compass.MinSatID && satelliteId <= _compass.MaxSatID)
                return _compass;
            else
                return null;
        }
    }
}
