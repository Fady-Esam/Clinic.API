using AutoMapper;
using Clinic.API.API.Dtos.RoleClaimDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Clinic.API.DL;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using static StackExchange.Redis.Role;

namespace Clinic.API.BL.Services
{
    public class RoleClaimsService : IRoleClaimsService
    {
        private readonly IRoleClaimsRepository _repo;
        private readonly IMapper _mapper;

        public RoleClaimsService(IRoleClaimsRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<RoleClaimDto>> AddRoleClaimAsync(CreateRoleClaimDto dto)
        {
            var newClaim = _mapper.Map<IdentityRoleClaim<string>>(dto);
            var claim = await _repo.AddRoleClaimAsync(newClaim);
            var mapped = _mapper.Map<RoleClaimDto>(claim);
            return ApiResponse<RoleClaimDto>.Success(mapped, "Role claim created successfully");
        }

        public async Task<ApiResponse<RoleClaimDto>> UpdateRoleClaimAsync(int id, UpdateRoleClaimDto dto)
        {
            var claim = await _repo.GetByIdAsync(id);
            if(claim == null)
                return ApiResponse<RoleClaimDto>.Failure("Failed to update role claim",
                    new() { "Role claim not found" }, StatusCodes.Status404NotFound);
            _mapper.Map(dto, claim);
            var updated = await _repo.UpdateRoleClaimAsync(claim);
            if (claim == null)
                return ApiResponse<RoleClaimDto>.Failure("Failed to update role claim",
                    new() { "Role claim not found" }, StatusCodes.Status404NotFound);

            var mapped = _mapper.Map<RoleClaimDto>(updated);
            return ApiResponse<RoleClaimDto>.Success(mapped, "Role claim updated successfully");
        }

        public async Task<ApiResponse<object>> DeleteRoleClaimAsync(int id)
        {
            var success = await _repo.DeleteRoleClaimAsync(id);
            if (!success)
                return ApiResponse<object>.Failure("Failed to delete role claim",
                    new() { "Role claim not found" }, StatusCodes.Status404NotFound);

            return ApiResponse<object>.Success(true, "Role claim deleted successfully");
        }

        public async Task<ApiResponse<RoleClaimDto>> GetByIdAsync(int id)
        {
            var claim = await _repo.GetByIdAsync(id);
            if (claim == null)
                return ApiResponse<RoleClaimDto>.Failure("Role claim not found",
                    new() { "Role claim not found" }, StatusCodes.Status404NotFound);

            return ApiResponse<RoleClaimDto>.Success(_mapper.Map<RoleClaimDto>(claim), "Role claim fetched successfully");
        }

        public async Task<ApiResponse<IReadOnlyList<RoleClaimDto>>> GetByRoleNameAsync(string roleName)
        {
            var claims = await _repo.GetByRoleNameAsync(roleName);
            return ApiResponse<IReadOnlyList<RoleClaimDto>>.Success(_mapper.Map<IReadOnlyList<RoleClaimDto>>(claims), "Role claims fetched successfully");
        }

        public async Task<ApiResponse<IReadOnlyList<RoleClaimDto>>> GetByRoleIdAsync(string roleId)
        {
            var claims = await _repo.GetByRoleIdAsync(roleId);
            return ApiResponse<IReadOnlyList<RoleClaimDto>>.Success(_mapper.Map<IReadOnlyList<RoleClaimDto>>(claims), "Role claims fetched successfully");
        }

        public async Task<ApiResponse<IReadOnlyList<RoleClaimDto>>> GetAllAsync()
        {
            var claims = await _repo.GetAllAsync();
            return ApiResponse<IReadOnlyList<RoleClaimDto>>.Success(_mapper.Map<IReadOnlyList<RoleClaimDto>>(claims), "All role claims fetched successfully");
        }
    }

}
