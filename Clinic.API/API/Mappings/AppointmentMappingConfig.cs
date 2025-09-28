using AutoMapper;
using Clinic.API.API.Dtos.AppointmentDtos;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;

namespace Clinic.API.API.Mappings
{
    public class AppointmentMappingConfig
    {
        public void Configure(Profile profile)
        {
            profile.CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.DoctorDto,
                           opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.PatientDto,
                           opt => opt.MapFrom(src => src.Patient))
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString()));

            profile.CreateMap<CreateAppointmentDto, Appointment>()
                .ForMember(dest => dest.CreatedAt,
                           opt => opt.MapFrom(_ => DateTime.UtcNow));


            profile.CreateMap<UpdateAppointmentDto, Appointment>()
                .ForMember(dest => dest.UpdatedAt,
                           opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
