using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;
using Clinic.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.DL.Repositories
{
    public class UserConfirmationCodeRepository : IUserConfirmationCodeRepository
    {
        private readonly ApplicationDbContext _context;

        public UserConfirmationCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserConfirmationCode confirmation)
        {
            _context.UserConfirmationCodes.Add(confirmation);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<UserConfirmationCode?> GetLatestAsync(string userId, ConfirmationType type, string code)
        {
            return await _context.UserConfirmationCodes
                .Where(c => c.UserId == userId && c.Type == type && c.Code == code && !c.IsUsed)
                .OrderByDescending(c => c.ExpireAt)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }
    }

}
