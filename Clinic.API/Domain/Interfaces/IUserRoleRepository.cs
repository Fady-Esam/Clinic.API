using Clinic.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Clinic.API.Domain.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<ApplicationUser?> FindUserByIdAsync(string userId);
        Task<IdentityRole?> FindRoleByIdAsync(string roleId);
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName);
        Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string roleName);
        Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles);
        Task<IReadOnlyList<IdentityUserRole<string>>> GetAllUserRolesAsync();
    }

}
