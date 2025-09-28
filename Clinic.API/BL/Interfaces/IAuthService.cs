using Clinic.API.API.Dtos.AuthDtos;
using Clinic.API.API.Dtos.AuthDtos.PasswordDtos;
using Clinic.API.API.Dtos.AuthDtos.RefreshTokenDtos;
using Clinic.API.Common.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Clinic.API.BL.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<ApiResponse<AuthResponseDto>> GoogleLoginAsync(string idToken);
        Task<ApiResponse<AuthResponseDto>> FacebookLoginAsync(string accessToken);
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(CreateNewRefreshTokenDto dto);
        Task<ApiResponse<object>> RevokeRefreshTokenAsync(RevokeRefreshTokenDto dto);
        Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordDto dto);
        Task<ApiResponse<object>> RequestPasswordResetAsync(PasswordResetRequestDto dto);
        Task<ApiResponse<object>> ConfirmPasswordResetAsync(PasswordResetConfirmDto dto);
        Task<ApiResponse<object>> ConfirmSmsAsync(SmsConfirmDto dto);
        Task<ApiResponse<object>> ConfirmEmailAsync(EmailConfirmDto dto);

    }

}
