using Clinic.API.API.Dtos.UserRoleDtos;
using Clinic.API.Common.Responses;
using Microsoft.AspNetCore.Identity;

namespace Clinic.API.BL.Interfaces
{
    public interface IUserRoleService
    {
        Task<ApiResponse<object>> AddRolesToUsersAsync(CreateUserRoleDto dto);
        Task<ApiResponse<object>> UpdateUserRolesAsync(UpdateUserRoleDto dto);
        Task<ApiResponse<object>> RemoveRolesFromUsersAsync(CreateUserRoleDto dto);
        //Task<ApiResponse<UserRolesResponseDto>> GetUserRolesAsync(string userId);
        Task<ApiResponse<UserRoleDto>> GetUserRolesAsync(string userId);
        Task<ApiResponse<IReadOnlyList<UserRoleDto>>> GetAllUserRolesAsync();
    }
}
