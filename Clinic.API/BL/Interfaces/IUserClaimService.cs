using Clinic.API.API.Dtos.UserClaimDtos;
using Clinic.API.Common.Responses;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Clinic.API.BL.Interfaces
{
    public interface IUserClaimService
    {
        Task<ApiResponse<UserClaimDto>> AddAsync(CreateUserClaimDto dto);
        Task<ApiResponse<UserClaimDto>> UpdateAsync(int id, UpdateUserClaimDto dto);
        Task<ApiResponse<object>> DeleteAsync(int id);
        Task<ApiResponse<UserClaimDto>> GetByIdAsync(int id);
        Task<ApiResponse<IReadOnlyList<UserClaimDto>>> GetByUserIdAsync(string userId);
        Task<ApiResponse<IReadOnlyList<UserClaimDto>>> GetAllAsync();
    }

}
