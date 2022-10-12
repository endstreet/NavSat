using AutoMapper;

namespace NavSat.Core.ApiClients.Profiles
{
    public class GeoJsonProfile : Profile
    {
        public GeoJsonProfile()
        {
            //CreateMap<SatellitePath, FeatureCollection>()
            //.ForMember(
            //        dest => dest.type,
            //        opt => opt.MapFrom("FeatureCollection")
            // );
        }
    }

    public class FeatureProfile : Profile
    {
        public FeatureProfile()
        {
            //CreateMap<SatelliteLocation, Feature>()
            //.ForMember(
            //        dest => dest.type,
            //        opt => opt.MapFrom("Point")
            // );
        }
    }
}
