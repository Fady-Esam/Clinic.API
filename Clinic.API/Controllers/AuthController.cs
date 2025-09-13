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


            var response = await _authService.RegisterAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString());

            return StatusCode(response.StatusCode, response);

            //if (result.StatusCode == StatusCodes.Status400BadRequest)
            //    return BadRequest(result);
            //return Ok(result);

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
                    Message = "Login Failed",
                    StatusCode = StatusCodes.Status400BadRequest
                });

            }

            var response = await _authService.LoginAsync(dto, HttpContext.Connection.RemoteIpAddress?.ToString());
            return StatusCode(response.StatusCode, response);

            //if (response.StatusCode == StatusCodes.Status401Unauthorized)
            //    return Unauthorized(response);

            //return Ok(response);

        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return BadRequest(new ApiResponse<AuthResponseDto>
                {
                    Errors = errors,
                    Message = "Getting new refresh tokeng failed",
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            dto.CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var response = await _authService.RefreshTokenAsync(dto);
            return StatusCode(response.StatusCode, response);

            //if (response.StatusCode == StatusCodes.Status400BadRequest)
            //    return BadRequest(response);
            //if (response.StatusCode == StatusCodes.Status401Unauthorized)
            //    return Unauthorized(response);
            //if (response.StatusCode == StatusCodes.Status404NotFound)
            //    return NotFound(response);
            //return Ok(response);
        }

        [Authorize]
        [HttpPost("revokeToken")]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenDto dto)
        {

            var response = await _authService.RevokeRefreshTokenAsync(dto);
            return StatusCode(response.StatusCode, response);

            //if (response.StatusCode == StatusCodes.Status404NotFound)
            //    return NotFound(response);
            //return Ok(response);
        }
    }
}

