using AutoMapper;
using Clinic.API.API.Dtos.RoleClaimDtos;
using Microsoft.AspNetCore.Identity;

namespace Clinic.API.API.Mappings
{
    public class RoleClaimMappingConfig
    {
        public void Configure(Profile profile)
        {
            profile.CreateMap<IdentityRoleClaim<string>, RoleClaimDto>();
            profile.CreateMap<CreateRoleClaimDto, IdentityRoleClaim<string>>();
            profile.CreateMap<UpdateRoleClaimDto, IdentityRoleClaim<string>>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
