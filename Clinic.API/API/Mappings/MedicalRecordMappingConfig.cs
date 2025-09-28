using AutoMapper;
using Clinic.API.API.Dtos.AppointmentDtos;
using Clinic.API.API.Dtos.MedicalRecordDtos;
using Clinic.API.Domain.Entities;

namespace Clinic.API.API.Mappings
{
    public class MedicalRecordMappingConfig
    {
        public void Configure(Profile profile)
        {
            profile.CreateMap<MedicalRecord, MedicalRecordDto>()
            .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient)) // Assuming Patient has a FullName property
            .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor));  // Assuming Doctor has a FullName property

            profile.CreateMap<CreateMedicalRecordDto, MedicalRecord>()
                .ForMember(dest => dest.RecordDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            profile.CreateMap<UpdateMedicalRecordDto, MedicalRecord>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}



