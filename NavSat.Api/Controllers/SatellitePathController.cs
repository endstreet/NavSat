﻿using AutoMapper;
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
        // Date Format 2022-07-29T21:58:39
        [HttpGet("Locations/{forDate?}")]
        public async Task<IEnumerable<SatellitePath>> GetSateliteLocations(DateTime? forDate = null)
        {

            DateTimeOffset forDateOffset = forDate == null ? DateTimeOffset.Now : DateTime.SpecifyKind((DateTime)forDate, DateTimeKind.Utc);

            return (IEnumerable<SatellitePath>)await _satellitePathService.GetPathsAsAtAsync(forDateOffset);
        }

        // Date Format 2022-07-29T21:58:39
        [HttpGet("Visible/{atDate?}")]
        public async Task<IEnumerable<Satellite>> GetVisibleSatelites(DateTime? atDate = null)
        {
            DateTimeOffset forDateOffset = atDate == null ? DateTimeOffset.Now : DateTime.SpecifyKind((DateTime)atDate, DateTimeKind.Utc);

            return await _satellitePathService.GetPathsAsAtAsync(forDateOffset);
        }


        //"latitude": -54.84178023604217,
        //"longitude": 131.8674045921112,
        //"altitude": 20183468.315013845
        [HttpGet("Visible/{lon}/{lat}/{alt}/{atDate?}")]
        public async Task<IEnumerable<SatellitePath>> GetVisibleSatelitePaths(double lon, double lat, double alt, DateTime? atDate = null)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset forDateOffset = atDate == null ? DateTimeOffset.Now : DateTime.SpecifyKind((DateTime)atDate, DateTimeKind.Utc);

            return (IEnumerable<SatellitePath>)await _satellitePathService.GetAsSeenFromAsAtAsync(atLocation, forDateOffset);
        }


        //GeoJson Calls ----------------------------------------------------------------------------------------------------------

        // Date Format 2022-07-29T21:58:39
        [HttpGet("GeoLocations/{forDate?}")]
        public async Task<string> GetSateliteLocationsGj(DateTime? forDate = null)
        {

            DateTimeOffset forDateOffset = forDate == null ? DateTimeOffset.Now : DateTime.SpecifyKind((DateTime)forDate, DateTimeKind.Utc);

            var srcObject = await _satellitePathService.GetPathsAsAtAsync(forDateOffset);
            //Map to FeatureCollection
            var jObject = srcObject.Select(x => _mapper.Map<FeatureCollection>(x));
            //Convert to JSON
            return JsonSerializer.Serialize(jObject);
        }

        // Date Format 2022-07-29T21:58:39
        [HttpGet("GeoVisible/{atDate?}")]
        public async Task<IEnumerable<Satellite>> GetVisibleSatelitesGj(DateTime? atDate = null)
        {
            DateTimeOffset forDateOffset = atDate == null ? DateTimeOffset.Now : DateTime.SpecifyKind((DateTime)atDate, DateTimeKind.Utc);

            return await _satellitePathService.GetPathsAsAtAsync(forDateOffset);
        }


        //"latitude": -54.84178023604217,
        //"longitude": 131.8674045921112,
        //"altitude": 20183468.315013845
        [HttpGet("GeoVisible/{lon}/{lat}/{alt}/{atDate?}")]
        public async Task<string> GetVisibleSatelitePathsGj(double lon, double lat, double alt, [FromRoute] DateTime? atDate = null)
        {
            GeoCoordinate atLocation = new GeoCoordinate() { Altitude = alt, Longitude = lon, Latitude = lat };
            DateTimeOffset forDateOffset = atDate == null ? DateTimeOffset.Now : DateTime.SpecifyKind((DateTime)atDate, DateTimeKind.Utc);

            var srcObject = (IEnumerable<SatellitePath>)await _satellitePathService.GetAsSeenFromAsAtAsync(atLocation, forDateOffset);

            //Map to FeatureCollection
            var jObject = srcObject.Select(x => _mapper.Map<FeatureCollection>(x));
            //Convert to JSON
            return JsonSerializer.Serialize(jObject);
        }


    }
}
