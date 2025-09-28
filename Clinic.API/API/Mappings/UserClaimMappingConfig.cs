using AutoMapper;
using Clinic.API.API.Dtos.RoleClaimDtos;
using Clinic.API.API.Dtos.UserClaimDtos;
using Microsoft.AspNetCore.Identity;

namespace Clinic.API.API.Mappings
{
    public class UserClaimMappingConfig
    {
        public void Configure(Profile profile)
        {
            profile.CreateMap<IdentityUserClaim<string>, UserClaimDto>();
            profile.CreateMap<CreateUserClaimDto, IdentityUserClaim<string>>();
            profile.CreateMap<UpdateUserClaimDto, IdentityUserClaim<string>>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
