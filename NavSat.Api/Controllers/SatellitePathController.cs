using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;
using System.Text.Json;
//using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace NavSat.Api.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class SatellitePathController : ControllerBase
    {
        private readonly ISatellitePathService _satellitePathService;
        private readonly IMapper _mapper;
        public SatellitePathController(ISatellitePathService satellitePathService,IMapper mapper)
        {
            _satellitePathService = satellitePathService;
            _mapper = mapper;
        }
        /// <summary>
        /// Get all Satellites for Date
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        /// <param name="forDate"></param>
        /// <returns></returns>
        [HttpGet("Locations/{forDate?}")]
        public async Task<IEnumerable<SatellitePath>> GetSateliteLocations( DateTimeOffset? forDate = null)
        {

            DateTimeOffset forDateOffset = (DateTimeOffset)(forDate == null ? DateTimeOffset.Now : forDate);

            return (IEnumerable<SatellitePath>)await _satellitePathService.GetPathsAsAtAsync(forDateOffset);
        }

        /// <summary>
        /// Get all Visible satellites for location
        /// </summary>
        /// <param name="atDate"></param>
        /// <returns></returns>
        [HttpGet("Visible/{lon}/{lat}/{alt}/{atDate?}")]
        public async Task<IEnumerable<SatellitePath>> GetVisibleSatelites(double lon, double lat, double alt, DateTimeOffset? atDate = null)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset forDateOffset = (DateTimeOffset)(atDate == null ? DateTimeOffset.Now : atDate);

            return await _satellitePathService.GetAsSeenFromAsAtAsync(atLocation, forDateOffset);
        }


        /// <summary>
        /// Get all Visible satellites foa a location at a specific time
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        /// <param name="fromDate">Optional</param>
        /// <param name="toDate">Optional</param>
        /// <returns></returns>
        [HttpGet("Visible/{lon}/{lat}/{alt}/{fromDate?}/{toDate?}")]
        public async Task<IEnumerable<SatellitePath>> GetVisibleSatelitePaths(double lon, double lat, double alt, [FromRoute] DateTimeOffset? fromDate = null, [FromRoute] DateTimeOffset? toDate = null)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset fromDateOffset = (DateTimeOffset)(fromDate == null ? DateTimeOffset.Now : fromDate);
            DateTimeOffset toDateOffset = (DateTimeOffset)(toDate == null ? DateTimeOffset.Now : toDate);

            return (IEnumerable<SatellitePath>)await _satellitePathService.GetAsSeenFromDuringAsync(atLocation, new List<DateTimeOffset> { fromDateOffset, toDateOffset });
        }


        //GeoJson Calls ----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get all Satellites for Date (GeoJSON)
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        /// <param name="forDate"></param>
        [HttpGet("GeoLocations/{forDate?}")]
        public async Task<string> GetSateliteLocationsGj(DateTimeOffset? forDate = null)
        {

            DateTimeOffset forDateOffset = (DateTimeOffset)(forDate == null ? DateTimeOffset.Now : forDate);

            var srcObject = await _satellitePathService.GetPathsAsAtAsync(forDateOffset);
            //Map to FeatureCollection
            var jObject = srcObject.Select(x => _mapper.Map<FeatureCollection>(x));
            //Convert to JSON
            return JsonSerializer.Serialize(jObject);
        }

        /// <summary>
        /// Get all Visible satellites for location (GeoJSON)
        /// </summary>
        /// <param name="atDate"></param>
        [HttpGet("GeoVisible/{lon}/{lat}/{alt}/{atDate?}")]
        public async Task<string> GetVisibleSatelitesGj(double lon, double lat, double alt, DateTimeOffset? atDate = null)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset forDateOffset = (DateTimeOffset)(atDate == null ? DateTimeOffset.Now : atDate);

            var srcObject = await _satellitePathService.GetAsSeenFromAsAtAsync(atLocation, forDateOffset);

            //Map to FeatureCollection
            var jObject = srcObject.Select(x => _mapper.Map<FeatureCollection>(x));
            //Convert to JSON
            return JsonSerializer.Serialize(jObject);
        }


        /// <summary>
        /// Get all Visible satellites foa a location at a specific time (GeoJSON)
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        /// <param name="fromDate">Optional</param>
        /// <param name="toDate">Optional</param>
        [HttpGet("GeoVisible/{lon}/{lat}/{alt}/{fromDate?}/{toDate?}")]
        public async Task<string> GetVisibleSatelitePathsGj(double lon, double lat, double alt, [FromRoute] DateTimeOffset? fromDate = null, [FromRoute] DateTime? toDate = null)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset fromDateOffset = (DateTimeOffset)(fromDate == null ? DateTimeOffset.Now : fromDate);
            DateTimeOffset toDateOffset = toDate == null ? DateTimeOffset.Now : DateTime.SpecifyKind((DateTime)toDate, DateTimeKind.Utc);

            //var srcObject = (IEnumerable<SatellitePath>)await _satellitePathService.GetAsSeenFromAsAtAsync(atLocation, forDateOffset);

            var srcObject = (IEnumerable<SatellitePath>)await _satellitePathService.GetAsSeenFromDuringAsync(atLocation, new List<DateTimeOffset> { fromDateOffset, toDateOffset });

            //Map to FeatureCollection
            var jObject = srcObject.Select(x => _mapper.Map<FeatureCollection>(x));
            //Convert to JSON
            return JsonSerializer.Serialize(jObject);
        }


    }
}
