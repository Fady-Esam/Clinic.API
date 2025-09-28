using Clinic.API.API.Dtos.RoleClaimDtos;
using Microsoft.AspNetCore.Identity;

namespace Clinic.API.Domain.Interfaces
{
    public interface IRoleClaimsRepository
    {
        Task<IdentityRoleClaim<string>> AddRoleClaimAsync(IdentityRoleClaim<string> roleClaim);
        Task<IdentityRoleClaim<string>> UpdateRoleClaimAsync(IdentityRoleClaim<string> roleClaim);
        Task<bool> DeleteRoleClaimAsync(int id);
        Task<IdentityRoleClaim<string>?> GetByIdAsync(int id);
        Task<IReadOnlyList<IdentityRoleClaim<string>>> GetByRoleNameAsync(string roleName);
        Task<IReadOnlyList<IdentityRoleClaim<string>>> GetByRoleIdAsync(string roleId);
        Task<IReadOnlyList<IdentityRoleClaim<string>>> GetAllAsync();
    }

}
