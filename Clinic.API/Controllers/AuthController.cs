using Azure;
using Clinic.API.BL.Dtos.AuthDtos;
using Clinic.API.BL.Interfaces.AuthInterfaces;
using Clinic.API.DL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Google.Apis.Requests.BatchRequest;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        private List<string> GetModelErrors() => ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<AuthResponseDto>.Failure(
                    "Registration Failed",
                    GetModelErrors()
                ));

            var response = await _authService.RegisterAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString());
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<AuthResponseDto>.Failure(
                    "Login Failed",
                    GetModelErrors()
                ));

            var response = await _authService.LoginAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString());
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> Refresh([FromBody] CreateRefreshTokenDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<AuthResponseDto>.Failure(
                    "Getting new refresh token failed",
                    GetModelErrors()
                ));

            var response = await _authService.RefreshTokenAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString());
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPost("revokeToken")]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto dto)
        {
            var response = await _authService.RevokeRefreshTokenAsync(dto);
            return StatusCode(response.StatusCode, response);
        }
    }

}

