using AutoMapper;
using Clinic.API.API.Dtos.PatientDtos;
using Clinic.API.Domain.Entities;

namespace Clinic.API.API.Mappings
{
    public class PatientMappingConfig
    {
        public void Configure(Profile profile)
        {
            profile.CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.ApplicationUserDto,
                           opt => opt.MapFrom(src => src.ApplicationUser));

            profile.CreateMap<CreatePatientDto, Patient>()
                .ForMember(dest => dest.DateOfRegisteration,
                           opt => opt.MapFrom(_ => DateTime.UtcNow));

            profile.CreateMap<UpdatePatientDto, Patient>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
