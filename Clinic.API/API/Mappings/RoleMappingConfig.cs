using AutoMapper;
using Clinic.API.API.Dtos.RoleDtos;
using Microsoft.AspNetCore.Identity;

namespace Clinic.API.API.Mappings
{
    public class RoleMappingConfig
    {
        public void Configure(Profile profile)
        {
            profile.CreateMap<IdentityRole, RoleDto>();
            profile.CreateMap<CreateRoleDto, IdentityRole>()
                .ForMember(dest => dest.NormalizedName,
                           opt => opt.MapFrom(src => src.Name.ToUpper()))
                .ForMember(dest => dest.ConcurrencyStamp,
                           opt => opt.MapFrom(_ => Guid.NewGuid().ToString()));

            profile.CreateMap<UpdateRoleDto, IdentityRole>()
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
