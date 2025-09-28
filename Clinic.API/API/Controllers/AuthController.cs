using Azure;
using Clinic.API.API.Dtos.AuthDtos;
using Clinic.API.API.Dtos.AuthDtos.PasswordDtos;
using Clinic.API.API.Dtos.AuthDtos.RefreshTokenDtos;
using Clinic.API.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Twilio.Http;
using static Google.Apis.Requests.BatchRequest;

namespace Clinic.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {

            var response = await _authService.RegisterAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {

            var response = await _authService.LoginAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] SocialLoginDto dto)
        {
            var response = await _authService.GoogleLoginAsync(dto.Token);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("facebook-login")]
        public async Task<IActionResult> FacebookLogin([FromBody] SocialLoginDto dto)
        {
            var response = await _authService.FacebookLoginAsync(dto.Token);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> Refresh([FromBody] CreateNewRefreshTokenDto dto)
        {
            var response = await _authService.RefreshTokenAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        //[Authorize]
        [HttpPost("revokeToken")]
        public async Task<IActionResult> Revoke([FromBody] RevokeRefreshTokenDto dto)
        {
            var response = await _authService.RevokeRefreshTokenAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var response = await _authService.ChangePasswordAsync(dto);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPost("password-reset-request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto dto)
        {
            var response = await _authService.RequestPasswordResetAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("password-reset-confirm")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] PasswordResetConfirmDto dto)
        {
            var response = await _authService.ConfirmPasswordResetAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmDto dto)
        {
            var response = await _authService.ConfirmEmailAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("confirm-sms")]
        public async Task<IActionResult> ConfirmSms([FromBody] SmsConfirmDto dto)
        {
            var response = await _authService.ConfirmSmsAsync(dto);
            return StatusCode(response.StatusCode, response);
        }
    }

}

