using AutoMapper;
using NavSat.Core.Abstrations.Models;
using NavSat.Core.ApiClients.Dtos;

namespace NavSat.Core.ApiClients.Profiles
{
    public class SatOrbitProfile : Profile
    {

        private readonly double rhoalt = 57.295779513082;

        public SatOrbitProfile()
        {

            CreateMap<SatAlmanac, SatelliteOrbit>()
                .ForMember(
                    dest => dest.A0,
                    opt => opt.MapFrom(src => src.SatClockOffset)
                )
                .ForMember(
                    dest => dest.A1,
                    opt => opt.MapFrom(src => src.SatClockDrift)
                )
                .ForMember(
                    dest => dest.DeltaInclination,
                    opt => opt.MapFrom(src => (54 + src.Inclination) / rhoalt)
                )
                .ForMember(
                    dest => dest.Eccentricity,
                    opt => opt.MapFrom(src => src.Eccentricity)
                )
                .ForMember(
                    dest => dest.GpsSeconds,
                    opt => opt.MapFrom(src => src.TimeOfAlmanac)
                )
                .ForMember(
                    dest => dest.GpsWeek,
                    opt => opt.MapFrom(src => src.GpsWeek)
                )
                .ForMember(
                    dest => dest.HealthCode,
                    opt => opt.MapFrom(src => src.Health)
                )
                .ForMember(
                    dest => dest.M0,
                    opt => opt.MapFrom(src => src.MeanAnomaly / rhoalt)
                )
                .ForMember(
                    dest => dest.Omega,
                    opt => opt.MapFrom(src => src.Perigee / rhoalt)
                )
                .ForMember(
                    dest => dest.Omega0,
                    opt => opt.MapFrom(src => src.RightAscension / rhoalt)
                )
                .ForMember(
                    dest => dest.OmegaDot,
                    opt => opt.MapFrom(src => src.AscensionRate / rhoalt / 1000.0)
                )
                .ForMember(
                    dest => dest.RootOfSemiMajorAxis,
                    opt => opt.MapFrom(src => src.SemiMajorAxis * src.SemiMajorAxis)
                )
                .ForMember(
                    dest => dest.SatId,
                    opt => opt.MapFrom(src => src.SVN)
                );
        }
    }
}
