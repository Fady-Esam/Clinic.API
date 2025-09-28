using AutoMapper;
using Clinic.API.API.Dtos.RoleDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Clinic.API.DL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static StackExchange.Redis.Role;

namespace Clinic.API.BL.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleService(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
        {
            if (await _roleManager.RoleExistsAsync(dto.Name))
                return ApiResponse<RoleDto>.Failure(
                    "Failed to create role",
                    new() { "Role already exists" },
                    StatusCodes.Status409Conflict);

            var role = _mapper.Map<IdentityRole>(dto);
            var result = await _roleManager.CreateAsync(role);

            return result.Succeeded
                ? ApiResponse<RoleDto>.Success(
                    _mapper.Map<RoleDto>(role),
                    "Role created successfully",
                    StatusCodes.Status201Created)
                : ApiResponse<RoleDto>.Failure(
                    "Failed to create role",
                    result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<ApiResponse<RoleDto>> UpdateRoleAsync(string id, UpdateRoleDto dto)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return ApiResponse<RoleDto>.Failure(
                    "Failed to update role",
                    new() { "Role not found" },
                    StatusCodes.Status404NotFound);

            _mapper.Map(dto, role);
            var result = await _roleManager.UpdateAsync(role);

            return result.Succeeded
                ? ApiResponse<RoleDto>.Success(
                    _mapper.Map<RoleDto>(role),
                    "Role updated successfully")
                : ApiResponse<RoleDto>.Failure(
                    "Failed to update role",
                    result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<ApiResponse<object>> DeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return ApiResponse<object>.Failure(
                    "Failed to delete role",
                    new() { "Role not found" },
                    StatusCodes.Status404NotFound);

            var result = await _roleManager.DeleteAsync(role);

            return result.Succeeded
                ? ApiResponse<object>.SuccessNoData("Role deleted successfully")
                : ApiResponse<object>.Failure(
                    "Failed to delete role",
                    result.Errors.Select(e => e.Description).ToList());
        }

        public async Task<ApiResponse<IReadOnlyList<RoleDto>>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var mapped = _mapper.Map<IReadOnlyList<RoleDto>>(roles);

            return ApiResponse<IReadOnlyList<RoleDto>>.Success(
                mapped,
                "Roles retrieved successfully");
        }
    }

}
