using AutoMapper;
using Clinic.API.API.Dtos.UserClaimDtos;
using Clinic.API.API.Dtos.UserRoleDtos;
using Microsoft.AspNetCore.Identity;

namespace Clinic.API.API.Mappings
{
    public class UserRoleMappingConfig
    {
        public void Configure(Profile profile)
        {
            profile.CreateMap<IdentityUserRole<string>, UserRoleDto>();
            profile.CreateMap<CreateUserRoleDto, IdentityUserRole<string>>();
            profile.CreateMap<UpdateUserRoleDto, IdentityUserRole<string>>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
