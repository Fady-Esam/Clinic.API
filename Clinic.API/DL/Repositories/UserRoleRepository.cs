using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Interfaces;
using Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.DL.Repositories
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserRoleRepository(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<ApplicationUser?> FindUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<IdentityRole?> FindRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleName)
        {
             return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            return await _userManager.RemoveFromRoleAsync(user, roleName);

        }

        public async Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles)
        {
            return await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task<IReadOnlyList<IdentityUserRole<string>>> GetAllUserRolesAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }
    }

}
