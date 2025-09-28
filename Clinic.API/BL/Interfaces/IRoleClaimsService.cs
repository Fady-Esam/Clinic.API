using Clinic.API.API.Dtos.RoleClaimDtos;
using Clinic.API.Common.Responses;
using Microsoft.AspNetCore.Identity;

namespace Clinic.API.BL.Interfaces
{
    public interface IRoleClaimsService
    {
        Task<ApiResponse<RoleClaimDto>> AddRoleClaimAsync(CreateRoleClaimDto dto);
        Task<ApiResponse<RoleClaimDto>> UpdateRoleClaimAsync(int id, UpdateRoleClaimDto dto);
        Task<ApiResponse<object>> DeleteRoleClaimAsync(int id);
        Task<ApiResponse<RoleClaimDto>> GetByIdAsync(int id);
        Task<ApiResponse<IReadOnlyList<RoleClaimDto>>> GetByRoleNameAsync(string roleName);
        Task<ApiResponse<IReadOnlyList<RoleClaimDto>>> GetByRoleIdAsync(string roleId);
        Task<ApiResponse<IReadOnlyList<RoleClaimDto>>> GetAllAsync();
    }

}
