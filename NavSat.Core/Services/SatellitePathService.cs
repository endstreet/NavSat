using NavSat.Core.Abstrations.ApiClients;
using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NavSat.Core.Services {
    public class SatellitePathService : ISatellitePathService {
        private readonly IGeoMath _geoMath;
        private readonly IOrbitApiClient _orbitApiClient;
        private readonly ISatMath _satMath;
        private readonly ISatelliteService _satService;

        public SatellitePathService(IGeoMath geoMath, ISatMath satMath, IOrbitApiClient orbitApiClient, ISatelliteService satService) {
            this._geoMath = geoMath;
            this._satMath = satMath;
            this._orbitApiClient = orbitApiClient;
            this._satService = satService;
        }

        public async Task<IEnumerable<SatellitePath>> GetPathsAsAtAsync(DateTimeOffset at) {

            var result = new List<SatellitePath>();

            var orbits = await _orbitApiClient.GetOrbitsAsAtAsync(at);

            foreach (var orbit in orbits) {

                var sat = _satService.CreateFrom(orbit.SatId);
                if (sat != null) {

                    var ecef = _satMath.CalculateEcef(at, orbit);

                    double lat, lon, height;

                    _geoMath.Ecef2Geo(ecef.ToXyzArray(), out lat, out lon, out height);

                    var geoCoord = GeoCoordinate.From(_geoMath.Rad2Deg(lat), _geoMath.Rad2Deg(lon), height);
                    var satPos = new SatelliteLocation(at, geoCoord);

                    result.Add(new SatellitePath(sat, orbit, satPos));

                }
            }

            return result;

        }

        public async Task<IEnumerable<SatellitePath>> GetAsSeenFromAsAtAsync(GeoCoordinate from, DateTimeOffset at) {
            var orbits = await _orbitApiClient.GetOrbitsAsAtAsync(at);
            return FilterAsSeenFromDuring(from, new DateTimeOffset[] { at }, orbits);
        }

        public async Task<IEnumerable<SatellitePath>> GetAsSeenFromDuringAsync(GeoCoordinate from, IEnumerable<DateTimeOffset> times) {

            var at = times.Min();
            var orbits = await _orbitApiClient.GetOrbitsAsAtAsync(at);
            return FilterAsSeenFromDuring(from, times, orbits);

        }

        protected internal virtual IEnumerable<SatellitePath> FilterAsSeenFromDuring(GeoCoordinate from, IEnumerable<DateTimeOffset> times, IEnumerable<SatelliteOrbit> orbits) {

            var result = new List<SatellitePath>();

            foreach (var orbit in orbits) {

                var sat = _satService.CreateFrom(orbit.SatId);
                if (sat != null) {

                    var positions = new List<SatelliteLocation>();

                    foreach (var time in times.OrderBy(t => t.Ticks)) {

                        var ecef = _satMath.CalculateEcef(time, orbit);

                        _geoMath.Ecef2Geo(ecef.ToXyzArray(), out double lat, out double lon, out double height);

                        var geoCoord = GeoCoordinate.From(_geoMath.Rad2Deg(lat), _geoMath.Rad2Deg(lon), height);

                        var stat = _geoMath.Geo2Ecef(
                                        _geoMath.Deg2Rad(from.Latitude),
                                        _geoMath.Deg2Rad(from.Longitude),
                                        from.Altitude);

                        var elAz = _geoMath.EcefToElAz(stat, ecef.ToXyzArray());

                        var relPos = SkyPlotCoordinate.From(elAz.Select(x => _geoMath.Rad2Deg(x)).ToArray());

                        var subjectivePos = new ObservedSatelliteLocation(time, geoCoord, relPos);

                        //TODO: Cutoff?

                        if (subjectivePos.SkyCoordinate.Elevation >= 10) {
                            //result.Add(new SatellitePath(sat, orbit, subjectivePos));
                            positions.Add(subjectivePos);
                        }

                    }

                    if (positions.Any()) {
                        result.Add(new SatellitePath(sat, orbit, positions));
                    }

                }


            }

            return result;

        }


    }
}
