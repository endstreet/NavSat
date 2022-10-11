using Microsoft.AspNetCore.Mvc;
using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;
using NavSat.Core.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NavSat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SatellitePathController : ControllerBase
    {
        private readonly ISatellitePathService _satellitePathService;
        public SatellitePathController(ISatellitePathService satellitePathService)
        {
            _satellitePathService = satellitePathService;
        }
        // Date Format 2022-07-29T21:58:39
        [HttpGet("Locations/{forDate:DateTime?}")]
        public async Task<IEnumerable<SatellitePath>> GetSateliteLocations(DateTime? forDate = null)
        {
 
            DateTimeOffset forDateOffset = forDate == null ? DateTimeOffset.Now : DateTime.SpecifyKind((DateTime)forDate, DateTimeKind.Utc);

            return (IEnumerable<SatellitePath>)await _satellitePathService.GetPathsAsAtAsync(forDateOffset);
        }

        // Date Format 2022-07-29T21:58:39
        [HttpGet("Visible/{atDate:DateTime?}")]
        public async Task<IEnumerable<Satellite>> GetVisibleSatelites(DateTime? atDate = null)
        {
            DateTimeOffset forDateOffset = atDate == null ? DateTimeOffset.Now : DateTime.SpecifyKind((DateTime)atDate, DateTimeKind.Utc);

            return (IEnumerable<Satellite>)await _satellitePathService.GetPathsAsAtAsync(forDateOffset);
        }


        //"latitude": -54.84178023604217,
        //"longitude": 131.8674045921112,
        //"altitude": 20183468.315013845
        [HttpGet("Visible/{lon}/{lat}/{alt}/{atDate}")]
        public async Task<IEnumerable<SatellitePath>> GetVisibleSatelitePaths(double lon,double lat, double alt, DateTime? atDate = null)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset forDateOffset = atDate == null ? DateTimeOffset.Now : DateTime.SpecifyKind((DateTime)atDate, DateTimeKind.Utc);

            return (IEnumerable<SatellitePath>)await _satellitePathService.GetAsSeenFromAsAtAsync(atLocation, forDateOffset);
        }
       
    }
}
