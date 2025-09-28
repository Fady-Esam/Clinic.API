using Clinic.API.API.Dtos.RoleDtos;
using Clinic.API.Common.Responses;
using Microsoft.AspNetCore.Identity;

namespace Clinic.API.BL.Interfaces
{
    public interface IRoleService
    {
        Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleDto dto);
        Task<ApiResponse<RoleDto>> UpdateRoleAsync(string id, UpdateRoleDto dto);
        Task<ApiResponse<object>> DeleteRoleAsync(string id);
        Task<ApiResponse<IReadOnlyList<RoleDto>>> GetAllRolesAsync();
    }
}
