using AutoMapper;
using Clinic.API.API.Dtos.DoctorDtos;
using Clinic.API.Domain.Entities;

namespace Clinic.API.API.Mappings
{
    public class DoctorMappingConfig
    {
        public void Configure(Profile profile)
        {
            profile.CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.ApplicationUserDto,
                           opt => opt.MapFrom(src => src.ApplicationUser));

            profile.CreateMap<CreateDoctorDto, Doctor>()
                .ForMember(dest => dest.DateOfRegisteration,
                           opt => opt.MapFrom(_ => DateTime.UtcNow));

            profile.CreateMap<UpdateDoctorDto, Doctor>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
