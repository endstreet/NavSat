using NavSat.Core.Abstrations.Models;
using System;

namespace NavSat.Core.Abstrations.Services {
    public interface ISatMath {

        EcefCoordinate CalculateEcef(DateTimeOffset dateTimeOffset, SatelliteOrbit orbit);

    }
}
