using AutoMapper;
using Clinic.API.BL.Dtos;
using Clinic.API.Domain.Entities;

namespace Clinic.API.BL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            //CreateMap<Patient, PatientDto>()
            //     .ReverseMap()
            //     .ForMember(dest => dest.ApplicationUser, opt => opt.Ignore()); // prevent EF overwrite
            CreateMap<Patient, PatientDto>()
                 .ReverseMap()
                 ; // prevent EF overwrite
        }
    }
}
