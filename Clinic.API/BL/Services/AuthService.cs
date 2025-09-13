using Azure;
using Azure.Core;
using Clinic.API.BL.Dtos.AuthDtos;
using Clinic.API.BL.Interfaces.AuthInterfaces;
using Clinic.API.DL.Models;
using Clinic.API.Domain.Entities;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop.Infrastructure;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Clinic.API.BL.Services
{
    public class AuthService : IAuthService
    {
        
       private readonly UserManager<ApplicationUser> _userManager;
       private readonly RoleManager<IdentityRole> _roleManager;
       private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, JWT jwt)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt;

        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto, string? ipAddress)
        {

            if(string.IsNullOrWhiteSpace(dto.RoleName))
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Message = "Registration failed",
                    Errors = new() { "Role Name must not be Empty or null or white spaces" },
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
          
            if (!await _roleManager.RoleExistsAsync(dto.RoleName))
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Message = "Registration failed",
                    Errors = new() { $"Role '{dto.RoleName}' does not exist in the system." },
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            var user = new ApplicationUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Message = "Registration failed",
                    Errors =  result.Errors.Select(e => e.Description).ToList(),
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            await _userManager.AddToRoleAsync(user, dto.RoleName);
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var accessToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            refreshToken.UserId = user.Id;
            refreshToken.CreatedByIp = ipAddress;
            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", JsonSerializer.Serialize(refreshToken));

            var authResponseDtoData = new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken.RefreshToken,
                Roles = roles,
                AccessTokenExpiration = accessToken.ValidTo,
                Message = "User registered successfully",
                StatusCode = StatusCodes.Status201Created
            };
            return new ApiResponse<AuthResponseDto>
            {
                Data = authResponseDtoData,
                Message = "User registered successfully",
                StatusCode = StatusCodes.Status201Created
            };

        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, string? ipAddress)
        {

            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Message = "Login failed",
                    Errors = new () { "Invalid username or password" },
                    StatusCode = StatusCodes.Status401Unauthorized,
                };
            }


            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var accessToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            refreshToken.UserId = user.Id;
            refreshToken.CreatedByIp = ipAddress;

            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", JsonSerializer.Serialize(refreshToken));

            var authResponseDtoData =  new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken.RefreshToken,
                Roles = roles,
                AccessTokenExpiration = accessToken.ValidTo,
                RefreshTokenExpiration = refreshToken.Expires,
                Message = "Login successful",
                StatusCode = StatusCodes.Status200OK,
            };
            return new ApiResponse<AuthResponseDto>
            {
                Data = authResponseDtoData,
                Message = "Login successful",
                StatusCode = StatusCodes.Status200OK,
            };

        }

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
        {

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Message = "Getting new refresh token failed",
                    Errors = new() { "User not found" },
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            // Get stored refresh token
            var storedTokenValue = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            if (string.IsNullOrEmpty(storedTokenValue))
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Message = "Getting new refresh token failed",
                    Errors = new() { "No refresh token found" },
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            var storedToken = JsonSerializer.Deserialize<RefreshTokenDto>(storedTokenValue);

            // Validate token
            if (storedToken == null)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Message = "Getting new refresh token failed",
                    Errors = new() { "Stored refresh token not found" },
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            if (storedToken.RefreshToken != dto.RefreshToken)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Message = "Getting new refresh token failed",    
                    Errors = new() { "Refresh token mismatch" },
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            if (storedToken.IsExpired)
            {
                return new ApiResponse<AuthResponseDto>
                {
                    Message = "Getting new refresh token failed",
                    Errors = new() { "Refresh token has expired" },
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }


            // ✅ Generate new tokens
            var newAccessToken = await GenerateJwtToken(user);
            var newRefreshToken = new RefreshTokenDto
            {
                UserId = user.Id,
                RefreshToken = GenerateRefreshToken().RefreshToken,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                CreatedByIp = dto.CreatedByIp,
            };

            // Mark old token revoked
            //storedToken.Revoked = DateTime.UtcNow;
            //storedToken.RevokedByIp = ipAddress;
            //storedToken.ReplacedByToken = newRefreshToken.RefreshToken;

            // Save new token
            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", JsonSerializer.Serialize(newRefreshToken));

            var authResponseDto =  new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken.RefreshToken,
                AccessTokenExpiration = newAccessToken.ValidTo,
                RefreshTokenExpiration = newRefreshToken.Expires!.Value,
                Message = "Token refreshed successfully",
                StatusCode = StatusCodes.Status200OK
            };
            return new ApiResponse<AuthResponseDto>
            {
                Data = authResponseDto,
                Message = "Token refreshed successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }


        public async Task<ApiResponse<bool>> RevokeRefreshTokenAsync(RevokeTokenDto dto)
        {


            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
            {
                return new ApiResponse<bool>
                {
                    Data = false,
                    Message = "User Not Found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            return new ApiResponse<bool>
            {
                Data = true,
                Message = "Refresh Token Revoked Successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }

        // 🔹 Helpers
        private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            };
            var roles = await _userManager.GetRolesAsync(user!);
            foreach (var item in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credintial = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                    issuer: _jwt.Issuer,
                    audience: _jwt.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(15),
                    signingCredentials: credintial
                );
            return jwtToken;

        }



        private RefreshTokenDto GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);

            return new RefreshTokenDto
            {
                RefreshToken = Convert.ToBase64String(randomBytes),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7) // long-lived
            };
        }
    }
}


