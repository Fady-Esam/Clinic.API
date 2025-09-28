using Clinic.API.API.Dtos.AuthDtos;
using Clinic.API.Domain.Entities;

namespace Clinic.API.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task AddAsync(RefreshToken token);
        Task Update(RefreshToken token);
        // Task SaveChangesAsync();
    }

}
