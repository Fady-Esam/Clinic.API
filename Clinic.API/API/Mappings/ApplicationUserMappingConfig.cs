using AutoMapper;
using Clinic.API.API.Dtos.ApplicationUserDtos;
using Clinic.API.API.Dtos.AuthDtos;
using Clinic.API.API.Dtos.UserClaimDtos;
using Clinic.API.Domain.Entities;
using System.Security.Claims;

namespace Clinic.API.API.Mappings
{
    public class ApplicationUserMappingConfig
    {
        public void Configure(Profile profile)
        {
            //profile.CreateMap<ApplicationUser, ApplicationUserDto>();
            //profile.CreateMap<RegisterDto, ApplicationUser>();
            //profile.CreateMap<UpdateApplicationUserDto, ApplicationUser>()
            //    .ForAllMembers(opts =>
            //        opts.Condition((src, dest, srcMember) => srcMember != null));
            profile.CreateMap<ApplicationUser, ApplicationUserDto>();
            profile.CreateMap<RegisterDto, ApplicationUser>();

            // This is an excellent way to handle updates, preventing overwrites with null values.
            profile.CreateMap<UpdateApplicationUserDto, ApplicationUser>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // ✨ NEW: Added mapping for Claims
            profile.CreateMap<Claim, ClaimDto>().ReverseMap();
        }
    }
}
