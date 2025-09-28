using Clinic.API.API.Dtos.AuthDtos;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Interfaces;
using Google;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.DL.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();

        }

        public async Task Update(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();

        }


    }

}
