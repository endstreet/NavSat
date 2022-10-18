using AutoMapper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NavSat.Core.Abstrations.Models;
using NavSat.Core.Abstrations.Services;
using System.Text.Json;


namespace NavSat.Api.Controllers
{

    /// <summary>
    /// SattlellitePath Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SatellitePathController : ControllerBase
    {
        private readonly ISatellitePathService _satellitePathService;
        private readonly IMapper _mapper;
        /// <summary>
        /// SattlellitePath Constructor
        /// </summary>
        /// <param name="satellitePathService"></param>
        /// <param name="mapper"></param>
        public SatellitePathController(ISatellitePathService satellitePathService, IMapper mapper)
        {
            _satellitePathService = satellitePathService;
            _mapper = mapper;
        }
        /// <summary>
        /// Get all Satellites for Date
        /// </summary>
        /// <param name="parameters">Optional</param>
        /// <returns></returns>
        [HttpGet("Locations/")]
        public async Task<IEnumerable<SatellitePath>> GetSateliteLocations([FromQuery] ApiforDateParameters parameters )
        {

            DateTimeOffset forDateOffset = (DateTimeOffset)(parameters.ForDate == null ? DateTimeOffset.Now : parameters.ForDate);

            return (IEnumerable<SatellitePath>)await _satellitePathService.GetPathsAsAtAsync(forDateOffset);
        }

        /// <summary>
        /// Get all Visible satellites for location/date
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        /// <param name="parameters">Optional</param>
        /// <returns></returns>
        [HttpGet("VisibleFrom/{lon}/{lat}/{alt}/")]
        public async Task<IEnumerable<SatellitePath>> GetVisibleSatelites(double lon, double lat, double alt, [FromQuery] ApiforDateParameters parameters)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset forDateOffset = (DateTimeOffset)(parameters.ForDate == null ? DateTimeOffset.Now : parameters.ForDate);

            return await _satellitePathService.GetAsSeenFromAsAtAsync(atLocation, forDateOffset);
        }


        /// <summary>
        /// Get all Visible satellites at a location for period
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        /// <param name="parameters">Optional</param>
        /// <returns></returns>
        [HttpGet("VisibleFromAt/{lon}/{lat}/{alt}/")]
        public async Task<IEnumerable<SatellitePath>> GetVisibleSatelitePaths(double lon, double lat, double alt, [FromQuery] ApiforPeriodParameters parameters)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset fromDateOffset = (DateTimeOffset)(parameters.FromDate == null ? DateTimeOffset.Now : parameters.FromDate);
            DateTimeOffset toDateOffset = (DateTimeOffset)(parameters.ToDate == null ? DateTimeOffset.Now : parameters.ToDate);

            return (IEnumerable<SatellitePath>)await _satellitePathService.GetAsSeenFromDuringAsync(atLocation, new List<DateTimeOffset> { fromDateOffset, toDateOffset });
        }


        //GeoJson Calls ----------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Get all Satellites for Date (GeoJSON)
        /// </summary>
        /// <param name="parameters">Optional</param>
        [HttpGet("GeoLocations/")]
        public async Task<string> GetSateliteLocationsGj([FromQuery] ApiforDateParameters parameters)
        {

            DateTimeOffset forDateOffset = (DateTimeOffset)(parameters.ForDate == null ? DateTimeOffset.Now : parameters.ForDate);

            var srcObject = await _satellitePathService.GetPathsAsAtAsync(forDateOffset);
            //Map to FeatureCollection
            var jObject = srcObject.Select(x => _mapper.Map<FeatureCollection>(x));
            //Convert to JSON
            return JsonSerializer.Serialize(jObject);
        }

        /// <summary>
        /// Get all Visible satellites for location/time (GeoJSON)
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        /// <param name="parameters">Optional</param>
        [HttpGet("GeoVisibleFrom/{lon}/{lat}/{alt}/")]
        public async Task<string> GetVisibleSatelitesGj(double lon, double lat, double alt, [FromQuery] ApiforDateParameters parameters)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset forDateOffset = (DateTimeOffset)(parameters.ForDate == null ? DateTimeOffset.Now : parameters.ForDate);

            var srcObject = await _satellitePathService.GetAsSeenFromAsAtAsync(atLocation, forDateOffset);

            //Map to FeatureCollection
            var jObject = srcObject.Select(x => _mapper.Map<FeatureCollection>(x));
            //Convert to JSON
            return JsonSerializer.Serialize(jObject);
        }


        /// <summary>
        /// Get all Visible satellites at a location for period (GeoJSON)
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <param name="alt"></param>
        /// <param name="parameters">Optional</param>
        [HttpGet("GeoVisibleFromAt/{lon}/{lat}/{alt}/")]
        public async Task<string> GetVisibleSatelitePathsGj(double lon, double lat, double alt, [FromQuery] ApiforPeriodParameters parameters)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset fromDateOffset = (DateTimeOffset)(parameters.FromDate == null ? DateTimeOffset.Now : parameters.FromDate);
            DateTimeOffset toDateOffset = (DateTimeOffset)(parameters.ToDate == null ? DateTimeOffset.Now : parameters.ToDate);

            var srcObject = (IEnumerable<SatellitePath>)await _satellitePathService.GetAsSeenFromDuringAsync(atLocation, new List<DateTimeOffset> { fromDateOffset, toDateOffset });

            //Map to FeatureCollection
            var jObject = srcObject.Select(x => _mapper.Map<FeatureCollection>(x));
            //Convert to JSON
            return JsonSerializer.Serialize(jObject);
        }

        /// <summary>
        /// Error endpoint
        /// </summary>
        /// <returns>Problem Details</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error")]
        public IActionResult HandleError() => Problem();

        /// <summary>
        /// Error endpoint
        /// </summary>
        /// <param name="hostEnvironment"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Route("/error-development")]
        public IActionResult HandleErrorDevelopment( [FromServices] IHostEnvironment hostEnvironment)
        {
            if (!hostEnvironment.IsDevelopment())
            {
                return NotFound();
            }

            var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;

            return Problem(
                detail: exceptionHandlerFeature.Error.StackTrace,
                title: exceptionHandlerFeature.Error.Message
                );
        }
    }
}
