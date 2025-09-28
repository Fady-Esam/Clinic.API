using AutoMapper;
using Clinic.API.API.Dtos.RoleClaimDtos;
using Clinic.API.API.Dtos.UserClaimDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Clinic.API.DL;
using Clinic.API.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.BL.Services
{
    public class UserClaimService : IUserClaimService
    {
        private readonly IUserClaimRepository _repo;
        private readonly IMapper _mapper;

        public UserClaimService(IUserClaimRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<UserClaimDto>> AddAsync(CreateUserClaimDto dto)
        {
            var newClaim = _mapper.Map<IdentityUserClaim<string>>(dto);
            var claim = await _repo.AddAsync(newClaim);

            return ApiResponse<UserClaimDto>.Success(
                _mapper.Map<UserClaimDto>(claim),
                "User claim added successfully",
                StatusCodes.Status201Created
            );
        }

        public async Task<ApiResponse<UserClaimDto>> UpdateAsync(int id, UpdateUserClaimDto dto)
        {

            var claim = await _repo.GetByIdAsync(id);
            if (claim == null)
                return ApiResponse<UserClaimDto>.Failure("Failed to update user claim",
                    new() { "User claim not found" }, StatusCodes.Status404NotFound);
            
            _mapper.Map(dto, claim);
            var updated = await _repo.UpdateAsync(claim);
            return ApiResponse<UserClaimDto>.Success(
                _mapper.Map<UserClaimDto>(updated),
                "User claim updated successfully"
            );
        }

        public async Task<ApiResponse<object>> DeleteAsync(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (!deleted)
                return ApiResponse<object>.Failure(
                    "Failed to delete user claim",
                    new() { "User claim not found" },
                    StatusCodes.Status404NotFound
                );

            return ApiResponse<object>.SuccessNoData("User claim deleted successfully");
        }

        public async Task<ApiResponse<UserClaimDto>> GetByIdAsync(int id)
        {
            var claim = await _repo.GetByIdAsync(id);
            if (claim == null)
                return ApiResponse<UserClaimDto>.Failure(
                    "Failed to retrieve claim",
                    new() { "Claim not found" },
                    StatusCodes.Status404NotFound
                );

            return ApiResponse<UserClaimDto>.Success(
                _mapper.Map<UserClaimDto>(claim),
                "Claim retrieved successfully"
            );
        }

        public async Task<ApiResponse<IReadOnlyList<UserClaimDto>>> GetByUserIdAsync(string userId)
        {
            var claims = await _repo.GetByUserIdAsync(userId);
            return ApiResponse<IReadOnlyList<UserClaimDto>>.Success(
                _mapper.Map<IReadOnlyList<UserClaimDto>>(claims),
                "User claims retrieved successfully"
            );
        }

        public async Task<ApiResponse<IReadOnlyList<UserClaimDto>>> GetAllAsync()
        {
            var claims = await _repo.GetAllAsync();
            return ApiResponse<IReadOnlyList<UserClaimDto>>.Success(
                _mapper.Map<IReadOnlyList<UserClaimDto>>(claims),
                "All claims retrieved successfully"
            );
        }
    }
}

