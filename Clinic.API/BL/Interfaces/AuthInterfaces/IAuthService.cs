
using Clinic.API.BL.Dtos.AuthDtos;
using Clinic.API.DL.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Clinic.API.BL.Interfaces.AuthInterfaces
{

    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto, string? ipAddress);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, string? ipAddress);
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(CreateRefreshTokenDto dto, string? createdByIp);
        Task<ApiResponse<bool>> RevokeRefreshTokenAsync(RevokeTokenDto dto);
    }
}
