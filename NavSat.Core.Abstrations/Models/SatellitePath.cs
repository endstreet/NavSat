using System.Collections.Generic;

namespace NavSat.Core.Abstrations.Models {
    public class SatellitePath : Satellite {

        public SatellitePath() { }

        public SatellitePath(Satellite baseSat, SatelliteOrbit orbit, SatelliteLocation position)
            : this(baseSat, orbit, new List<SatelliteLocation>() { position }) { }

        public SatellitePath(Satellite baseSat, SatelliteOrbit orbit, IEnumerable<SatelliteLocation> path) : base(baseSat.Id, baseSat.Prn, baseSat.Constellation) {
            this.Path = path;
            this.Orbit = orbit;
        }

        public IEnumerable<SatelliteLocation> Path { get; private set; }

        public SatelliteOrbit Orbit { get; private set; }

    }
}
