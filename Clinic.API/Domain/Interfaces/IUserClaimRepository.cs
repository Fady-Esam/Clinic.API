using Clinic.API.API.Dtos.UserClaimDtos;
using Microsoft.AspNetCore.Identity;

namespace Clinic.API.Domain.Interfaces
{
    public interface IUserClaimRepository
    {
        Task<IdentityUserClaim<string>> AddAsync(IdentityUserClaim<string> userClaim);
        Task<IdentityUserClaim<string>> UpdateAsync(IdentityUserClaim<string> userClaim);
        Task<bool> DeleteAsync(int id);
        Task<IdentityUserClaim<string>?> GetByIdAsync(int id);
        Task<IReadOnlyList<IdentityUserClaim<string>>> GetByUserIdAsync(string userId);
        Task<IReadOnlyList<IdentityUserClaim<string>>> GetAllAsync();
    }

}
