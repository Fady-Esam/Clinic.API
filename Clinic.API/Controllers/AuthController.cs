using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.DL.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid) 
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return BadRequest(new ApiResponse<AuthResponseDto>
                {
                    Errors = errors,
                    Message = "Registeration Failed",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }


            var result = await _authService.RegisterAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString());

            if (result.StatusCode == StatusCodes.Status400BadRequest)
                return BadRequest(result);
            else
            {
                //Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                //{
                //    HttpOnly = true,
                //    Secure = true,
                //    SameSite = SameSiteMode.Strict,
                //    Expires = DateTime.UtcNow.AddDays(7)
                //});

                return Ok(result);
            } 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return BadRequest(new ApiResponse<AuthResponseDto>
                {
                    Errors = errors,
                    Message = "Log in Failed",
                    StatusCode = StatusCodes.Status400BadRequest
                });

            }

            var response = await _authService.LoginAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString());
            if (response.StatusCode == StatusCodes.Status401Unauthorized)
                return Unauthorized(response);
            else
            {
                //Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
                //{
                //    HttpOnly = true,
                //    Secure = true,
                //    SameSite = SameSiteMode.Strict,
                //    Expires = DateTime.UtcNow.AddDays(7)
                //});
                return Ok(response);
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return BadRequest(new ApiResponse<AuthResponseDto>
                {
                    Errors = errors,
                    Message = "Refresh Failed",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var response = await _authService.RefreshTokenAsync(dto.UserId, dto.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString());
            if (response.StatusCode == StatusCodes.Status400BadRequest)
                return BadRequest(response);
            else if (response.StatusCode == StatusCodes.Status401Unauthorized)
                return Unauthorized(response);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto dto)
        {

            var success = await _authService.RevokeRefreshTokenAsync(dto.UserId);
            if (!success) return BadRequest(new { errors = "Failed to revoke token" });

            return Ok(new { message = "Refresh token revoked successfully" });
        }
    }
}

