using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Utils;
using System;
using System.Collections.Generic;

namespace NavSat.Core.Abstrations.Services
{
    public interface IAlmanacService
    {
        DateTime AlmanacTimeUTC { get; }
        IEnumerable<SatelliteOrbit> FullAlmanac { get; }

        void Add(IEnumerable<SatelliteOrbit> satAlms);
        void DefineCurtain(double[] curtain);
        IEnumerable<Satellite> GetDops(IEnumerable<Constellation> systems, IGPSTime time, double elev_cutoff, double[] pos, int[] availbaleSatIds, out double gDOP, out double tDOP, out double pDOP, out double hDOP, out double vDOP);
        SatelliteOrbit GetSatelliteById(int id);
        bool GetSatellitePosition(IGPSTime time, Satellite sat, double[] pos, out double az, out double el, out double[] xecf);
        IEnumerable<Satellite> GetSats(IEnumerable<Constellation> systems, IGPSTime time);
        IEnumerable<Satellite> GetVisibleSats(IEnumerable<Constellation> systems, IGPSTime time, double cutoff, double[] pos);
    }
}
