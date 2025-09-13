using AutoMapper;
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.DoctorDtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.Domain.Entities;

namespace Clinic.API.BL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>();

            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.ApplicationUserDto,
                           opt => opt.MapFrom(src => src.ApplicationUser)); // simplified

            CreateMap<CreateOrUpdatePatientDto, Patient>().ReverseMap();

            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.ApplicationUserDto,
                           opt => opt.MapFrom(src => src.ApplicationUser)); // simplified

            CreateMap<CreateOrUpdateDoctorDto, Doctor>().ReverseMap();
        }
    }
}
