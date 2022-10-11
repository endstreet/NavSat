using System.Collections.Generic;

namespace NavSat.Core.ApiClients.Dtos {
    public class FullAlmanacResponse {

        public long Time { get; set; }

        public List<SatAlmanac> Satellites { get; set; }

    }
}
