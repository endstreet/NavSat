﻿using AutoMapper;
using NavSat.Core.Abstrations.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml.Serialization;

namespace NavSat.Core.ApiClients.Profiles
{
    public class GeoJsonProfile : Profile
    {

        public GeoJsonProfile()
        {
            CreateMap<SatellitePath, FeatureCollection>()
            .ForMember(dest => dest.features, opt => opt.MapFrom(src => new List<Feature>()
            {
                new Feature(){
                    geometry = (src.Path.Count() == 1) ? new Geometry {type = "Point",coordinates = src.Path.Select(x =>  new List<double>{ x.Trace.Longitude, x.Trace.Latitude }).First() } : new Geometry { type = "LineString",coordinates = src.Path.Select(x =>  new List<double>{ x.Trace.Longitude, x.Trace.Latitude }).ToList() },
                    properties = new Properties
                    {
                        Name = src.DisplayName,
                        IsHealthy = src.Orbit.IsHealthy,
                        Constellation = src.Constellation
                    }
                }
            }));
        }

    }
}
