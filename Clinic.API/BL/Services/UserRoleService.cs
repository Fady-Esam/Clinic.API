using AutoMapper;
using Clinic.API.API.Dtos.UserRoleDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Clinic.API.DL;
using Clinic.API.Domain.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.ObjectPool;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clinic.API.BL.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _repo;
        private readonly IMapper _mapper;

        public UserRoleService(IUserRoleRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<object>> AddRolesToUsersAsync(CreateUserRoleDto dto)
        {
            const string errMessage = "Failed to add roles to users";

            foreach (var userId in dto.UserIds)
            {
                var user = await _repo.FindUserByIdAsync(userId);
                if (user == null)
                    return ApiResponse<object>.Failure(errMessage, new() { $"User {userId} not found" }, StatusCodes.Status404NotFound);

                var userRoles = await _repo.GetUserRolesAsync(user);

                foreach (var roleId in dto.RoleIds)
                {
                    var role = await _repo.FindRoleByIdAsync(roleId);
                    if (role == null)
                        return ApiResponse<object>.Failure(errMessage, new() { $"Role {roleId} not found" }, StatusCodes.Status404NotFound);

                    if (!string.IsNullOrEmpty(role.Name) && !userRoles.Contains(role.Name))
                    {
                        var result = await _repo.AddToRoleAsync(user, role.Name);
                        if (!result.Succeeded)
                            return ApiResponse<object>.Failure(errMessage, result.Errors.Select(e => e.Description).ToList());
                    }
                }
            }

            return ApiResponse<object>.SuccessNoData("Roles added to users successfully");
        }

        public async Task<ApiResponse<object>> UpdateUserRolesAsync(UpdateUserRoleDto dto)
        {
            const string errMessage = "Failed to update user roles";

            var user = await _repo.FindUserByIdAsync(dto.UserId);
            if (user == null)
                return ApiResponse<object>.Failure(errMessage, new() { $"User {dto.UserId} not found" }, StatusCodes.Status404NotFound);

            var currentRoles = await _repo.GetUserRolesAsync(user);
            if (currentRoles.Any())
            {
                var result = await _repo.RemoveFromRolesAsync(user, currentRoles);
                if (!result.Succeeded)
                    return ApiResponse<object>.Failure(errMessage, result.Errors.Select(e => e.Description).ToList());
            }

            foreach (var roleId in dto.RoleIds)
            {
                var role = await _repo.FindRoleByIdAsync(roleId);
                if (role == null)
                    return ApiResponse<object>.Failure(errMessage, new() { $"Role {roleId} not found" }, StatusCodes.Status404NotFound);

                if (!string.IsNullOrEmpty(role.Name))
                {
                    var result = await _repo.AddToRoleAsync(user, role.Name);
                    if (!result.Succeeded)
                        return ApiResponse<object>.Failure(errMessage, result.Errors.Select(e => e.Description).ToList());
                }
            }

            return ApiResponse<object>.SuccessNoData("User roles updated successfully");
        }

        public async Task<ApiResponse<object>> RemoveRolesFromUsersAsync(CreateUserRoleDto dto)
        {
            const string errMessage = "Failed to remove roles from users";

            foreach (var userId in dto.UserIds)
            {
                var user = await _repo.FindUserByIdAsync(userId);
                if (user == null)
                    return ApiResponse<object>.Failure(errMessage, new() { $"User {userId} not found" }, StatusCodes.Status404NotFound);

                var userRoles = await _repo.GetUserRolesAsync(user);

                foreach (var roleId in dto.RoleIds)
                {
                    var role = await _repo.FindRoleByIdAsync(roleId);
                    if (role == null)
                        return ApiResponse<object>.Failure(errMessage, new() { $"Role {roleId} not found" }, StatusCodes.Status404NotFound);

                    if (!string.IsNullOrEmpty(role.Name) && userRoles.Contains(role.Name))
                    {
                        var result = await _repo.RemoveFromRoleAsync(user, role.Name);
                        if (!result.Succeeded)
                            return ApiResponse<object>.Failure(errMessage, result.Errors.Select(e => e.Description).ToList());
                    }
                }
            }

            return ApiResponse<object>.SuccessNoData("Roles removed from users successfully");
        }

        public async Task<ApiResponse<UserRoleDto>> GetUserRolesAsync(string userId)
        {
            var user = await _repo.FindUserByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserRoleDto>.Failure("Failed to get user roles",
                    new() { $"User {userId} not found" }, StatusCodes.Status404NotFound);

            var roles = await _repo.GetUserRolesAsync(user);

            var result = new UserRoleDto
            {
                UserId = userId,
                Roles = roles.ToList()
            };

            return ApiResponse<UserRoleDto>.Success(result, "User roles retrieved successfully");
        }
        //public async Task<ApiResponse<UserRolesResponseDto>> GetUserRolesAsync(string userId)
        //{
        //    var user = await _repo.FindUserByIdAsync(userId);
        //    if (user == null)
        //        return ApiResponse<UserRolesResponseDto>.Failure("Failed to get user roles",
        //            new() { $"User {userId} not found" }, StatusCodes.Status404NotFound);

        //    var roles = await _repo.GetUserRolesAsync(user);

        //    var result = new UserRolesResponseDto
        //    {
        //        UserId = userId,
        //        Roles = roles.ToList()
        //    };

        //    return ApiResponse<UserRolesResponseDto>.Success(result, "User roles retrieved successfully");
        //}

        public async Task<ApiResponse<IReadOnlyList<UserRoleDto>>> GetAllUserRolesAsync()
        {
            var roles = await _repo.GetAllUserRolesAsync();
            var mapped = _mapper.Map<IReadOnlyList<UserRoleDto>>(roles);

            return ApiResponse<IReadOnlyList<UserRoleDto>>.Success(mapped, "All user roles retrieved successfully");
        }
    }

}
