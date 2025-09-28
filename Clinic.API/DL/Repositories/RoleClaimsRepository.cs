using Clinic.API.API.Dtos.RoleClaimDtos;
using Clinic.API.DL;
using Clinic.API.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.Repositories.RoleClaims
{
    public class RoleClaimsRepository : IRoleClaimsRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleClaimsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IdentityRoleClaim<string>> AddRoleClaimAsync(IdentityRoleClaim<string> roleClaim)
        {

            _context.RoleClaims.Add(roleClaim);
            await _context.SaveChangesAsync();
            return roleClaim;
        }

        public async Task<IdentityRoleClaim<string>> UpdateRoleClaimAsync(IdentityRoleClaim<string> roleClaim)
        {

            _context.RoleClaims.Update(roleClaim);
            await _context.SaveChangesAsync();
            return roleClaim;
        }

        public async Task<bool> DeleteRoleClaimAsync(int id)
        {
            var claim = await _context.RoleClaims.FindAsync(id);
            if (claim == null) return false;

            _context.RoleClaims.Remove(claim);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IdentityRoleClaim<string>?> GetByIdAsync(int id)
            => await _context.RoleClaims.FindAsync(id);

        public async Task<IReadOnlyList<IdentityRoleClaim<string>>> GetByRoleNameAsync(string roleName)
            => await (from rc in _context.RoleClaims
                      join r in _context.Roles on rc.RoleId equals r.Id
                      where r.Name == roleName
                      select rc).ToListAsync();

        public async Task<IReadOnlyList<IdentityRoleClaim<string>>> GetByRoleIdAsync(string roleId)
            => await _context.RoleClaims.Where(c => c.RoleId == roleId).ToListAsync();

        public async Task<IReadOnlyList<IdentityRoleClaim<string>>> GetAllAsync()
            => await _context.RoleClaims.ToListAsync();
    }

}
