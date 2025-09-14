using AutoMapper;
using Clinic.API.BL.Dtos.ApplicationUserDtos;
using Clinic.API.BL.Dtos.AppointmentDtos;
using Clinic.API.BL.Dtos.AuthDtos;
using Clinic.API.BL.Dtos.DoctorDtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;

namespace Clinic.API.BL.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

           
            CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            CreateMap<ApplicationUser, UpdateApplicationUserDto>().ReverseMap()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ================== PATIENT ==================
            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.ApplicationUserDto,
                           opt => opt.MapFrom(src => src.ApplicationUser))
                           .ReverseMap();

            CreateMap<CreatePatientDto, Patient>()
                .ForMember(dest => dest.DateOfRegisteration,
                           opt => opt.MapFrom(_ => DateTime.UtcNow)); // set registration date on creation

            CreateMap<UpdatePatientDto, Patient>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            //.ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore()) // prevent overwrite
            //.ForMember(dest => dest.DateOfRegisteration, opt => opt.Ignore()); // preserve original date

            // ================== DOCTOR ==================
            CreateMap<Doctor, DoctorDto>()
                .ForMember(dest => dest.ApplicationUserDto,
                           opt => opt.MapFrom(src => src.ApplicationUser))
                            .ReverseMap();
            CreateMap<CreateDoctorDto, Doctor>()
                .ForMember(dest => dest.DateOfRegisteration,
                           opt => opt.MapFrom(_ => DateTime.UtcNow)); // set registration date on creation

            CreateMap<UpdateDoctorDto, Doctor>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            //.ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore()) // prevent overwrite
            //.ForMember(dest => dest.DateOfRegisteration, opt => opt.Ignore()); // preserve original date


            // =========================  APPOINTMENT  =================================
            CreateMap<Appointment, AppointmentDto>()
                 .ForMember(dest => dest.DoctorDto,
                           opt => opt.MapFrom(src => src.Doctor))
                  .ForMember(dest => dest.PatientDto,
                           opt => opt.MapFrom(src => src.Patient))
                    .ReverseMap();
            CreateMap<CreateAppointmentDto, Appointment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => AppointmentStatus.Pending));
            //.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            //.ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<UpdateAppointmentDto, Appointment>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            // update timestamp
              //.ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // preserve original creation date
            //.ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        }
    }
}
