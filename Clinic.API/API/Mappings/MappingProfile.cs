using AutoMapper;
using Clinic.API.API.Dtos.ApplicationUserDtos;
using Clinic.API.API.Dtos.AppointmentDtos;
using Clinic.API.API.Dtos.DoctorDtos;
using Clinic.API.API.Dtos.PatientDtos;
using Clinic.API.API.Dtos.RoleClaimDtos;
using Clinic.API.API.Dtos.RoleDtos;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace Clinic.API.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            new ApplicationUserMappingConfig().Configure(this);
            new RoleMappingConfig().Configure(this);
            new RoleClaimMappingConfig().Configure(this);
            new PatientMappingConfig().Configure(this);
            new DoctorMappingConfig().Configure(this);
            new AppointmentMappingConfig().Configure(this);
            new UserRoleMappingConfig().Configure(this);
            new UserClaimMappingConfig().Configure(this);
            new RefreshTokenMappingConfig().Configure(this);
            new MedicalRecordMappingConfig().Configure(this);
            new InvoiceMappingConfig().Configure(this);


            //// ==================== Refresh Token ============================
            ////CreateMap<RefreshToken, RefreshTokenDto>()
            ////    .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => DateTime.UtcNow >= src.Expires))
            ////    .ForMember(dest => dest.IsRevoked, opt => opt.MapFrom(src => src.Revoked != null))
            ////    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => !src.IsExpired && !src.IsRevoked));

            //CreateMap<CreateNewRefreshTokenDto, RefreshToken>()
            //    .ForMember(dest => dest.Token, opt => opt.MapFrom(_ => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))))
            //    .ForMember(dest => dest.Expires, opt => opt.MapFrom(_ => DateTime.UtcNow.AddDays(7)));
            //CreateMap<RevokeRefreshTokenDto, RefreshToken>()
            //    .ForMember(dest => dest.Revoked, opt => opt.MapFrom(_ => DateTime.UtcNow));



            //// =================== End Refresh Token ==========================
            //CreateMap<ApplicationUser, ApplicationUserDto>().ReverseMap();
            //CreateMap<ApplicationUser, UpdateApplicationUserDto>()
            //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            //// ================= Roles =================
            //// Map CreateRoleDto -> IdentityRole
            //CreateMap<CreateRoleDto, IdentityRole>()
            //    .ForMember(dest => dest.NormalizedName,
            //               opt => opt.MapFrom(src => src.Name.ToUpper())) // auto set normalized name
            //    .ForMember(dest => dest.ConcurrencyStamp,
            //               opt => opt.MapFrom(_ => Guid.NewGuid().ToString())); // new stamp on creation

            //// Map UpdateRoleDto -> IdentityRole
            //CreateMap<UpdateRoleDto, IdentityRole>()
            //    .ForAllMembers(opts =>
            //        opts.Condition((src, dest, srcMember) => srcMember != null)); // only map non-null

            //// ================= Role Claims =================
            //CreateMap<CreateRoleClaimDto, IdentityRoleClaim<string>>();

            //CreateMap<UpdateRoleClaimDto, IdentityRoleClaim<string>>()
            //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            //// ================== PATIENT ==================

            //CreateMap<Patient, PatientDto>()
            //    .ForMember(dest => dest.ApplicationUserDto,
            //               opt => opt.MapFrom(src => src.ApplicationUser))
            //               .ReverseMap();

            //CreateMap<CreatePatientDto, Patient>()
            //    .ForMember(dest => dest.DateOfRegisteration,
            //               opt => opt.MapFrom(_ => DateTime.UtcNow)); // set registration date on creation

            //CreateMap<UpdatePatientDto, Patient>()
            //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            ////.ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore()) // prevent overwrite
            ////.ForMember(dest => dest.DateOfRegisteration, opt => opt.Ignore()); // preserve original date

            //// ================== DOCTOR ==================
            //CreateMap<Doctor, DoctorDto>()
            //    .ForMember(dest => dest.ApplicationUserDto,
            //               opt => opt.MapFrom(src => src.ApplicationUser))
            //                .ReverseMap();
            //CreateMap<CreateDoctorDto, Doctor>()
            //    .ForMember(dest => dest.DateOfRegisteration,
            //               opt => opt.MapFrom(_ => DateTime.UtcNow)); // set registration date on creation

            //CreateMap<UpdateDoctorDto, Doctor>()
            //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            ////.ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore()) // prevent overwrite
            ////.ForMember(dest => dest.DateOfRegisteration, opt => opt.Ignore()); // preserve original date


            //// =========================  APPOINTMENT  =================================
            //CreateMap<Appointment, AppointmentDto>()
            //     .ForMember(dest => dest.DoctorDto,
            //               opt => opt.MapFrom(src => src.Doctor))
            //      .ForMember(dest => dest.PatientDto,
            //               opt => opt.MapFrom(src => src.Patient))
            //        .ReverseMap();
            //CreateMap<CreateAppointmentDto, Appointment>()
            //    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => AppointmentStatus.Pending));
            ////.ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            ////.ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            //CreateMap<UpdateAppointmentDto, Appointment>()
            //    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            //    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            //// update timestamp
            ////.ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // preserve original creation date
            ////.ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

        }
    }

}


