using NavSat.Core.Abstrations.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NavSat.Core.Abstrations.Services
{
    public interface ISatellitePathService
    {

        Task<IEnumerable<SatellitePath>> GetPathsAsAtAsync(DateTimeOffset at);

        Task<IEnumerable<SatellitePath>> GetAsSeenFromAsAtAsync(GeoCoordinate from, DateTimeOffset at);

        Task<IEnumerable<SatellitePath>> GetAsSeenFromDuringAsync(GeoCoordinate from, IEnumerable<DateTimeOffset> times);

        Task<string> GetGeoJSONPathsAsAtAsync(DateTimeOffset at);

        Task<string> GetGeoJSONAsSeenFromAsAtAsync(GeoCoordinate from, DateTimeOffset at);

        Task<string> GetGeoJSONAsSeenFromDuringAsync(GeoCoordinate from, IEnumerable<DateTimeOffset> times);


    }
}
