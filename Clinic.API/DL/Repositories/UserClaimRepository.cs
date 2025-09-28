using AutoMapper;
using Clinic.API.API.Dtos.UserClaimDtos;
using Clinic.API.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Clinic.API.DL.Repositories
{
    public class UserClaimRepository : IUserClaimRepository
    {
        private readonly ApplicationDbContext _context;

        public UserClaimRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IdentityUserClaim<string>> AddAsync(IdentityUserClaim<string> userClaim)
        {
            _context.UserClaims.Add(userClaim);
            await _context.SaveChangesAsync();
            return userClaim;
        }

        public async Task<IdentityUserClaim<string>> UpdateAsync(IdentityUserClaim<string> userClaim)
        {
            _context.UserClaims.Update(userClaim);
            await _context.SaveChangesAsync();
            return userClaim;

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var claim = await _context.UserClaims.FindAsync(id);
            if (claim == null) return false;

            _context.UserClaims.Remove(claim);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IdentityUserClaim<string>?> GetByIdAsync(int id)
        {
            return await _context.UserClaims.FindAsync(id);
        }

        public async Task<IReadOnlyList<IdentityUserClaim<string>>> GetByUserIdAsync(string userId)
        {
            return await _context.UserClaims
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<IdentityUserClaim<string>>> GetAllAsync()
        {
            return await _context.UserClaims.ToListAsync();
        }
    }
}
