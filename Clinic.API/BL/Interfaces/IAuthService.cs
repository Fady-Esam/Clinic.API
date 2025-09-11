using Clinic.API.BL.Dtos;
using System.IdentityModel.Tokens.Jwt;

namespace Clinic.API.BL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string? ipAddress);
        Task<AuthResponseDto> LoginAsync(LoginDto dto, string? ipAddress);
        Task<AuthResponseDto>  RefreshTokenAsync(string userId, string refreshToken, string? ipAddress);
        Task<bool> RevokeRefreshTokenAsync(string userId);
    }
}
