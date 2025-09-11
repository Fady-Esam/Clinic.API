using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.DL.Models;
using Clinic.API.Domain.Identity;
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
       private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, JWT jwt)
        {
            _userManager = userManager;
            _jwt = jwt;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, string? ipAddress)
        {
            if (await _userManager.FindByEmailAsync(dto.Username) != null)
            {
                
                return new AuthResponseDto
                {
                    Errors = new() { "UserName already exists" },
                    Message = "Registration failed",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Username,
                FullName = dto.Username,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDto
                {
                    Errors =  result.Errors.Select(e => e.Description).ToList(),
                    Message = "Registration failed",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            await _userManager.AddToRoleAsync(user, "Patient");
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var accessToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            refreshToken.UserId = user.Id;
            refreshToken.CreatedByIp = ipAddress;
            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", JsonSerializer.Serialize(refreshToken));

            return new AuthResponseDto
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
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto, string? ipAddress)
        {
            
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new AuthResponseDto
                {
                    Errors = new () { "Invalid username or password" },
                    Message = "Login failed",
                    StatusCode = StatusCodes.Status401Unauthorized,
                };
            }


            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var accessToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            refreshToken.UserId = user.Id;
            refreshToken.CreatedByIp = ipAddress;

            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", JsonSerializer.Serialize(refreshToken));

            return new AuthResponseDto
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

        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string userId, string refreshToken, string? ipAddress)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResponseDto
                {
                    Errors = new() { "User not found" },
                    Message = "Refresh token validation failed",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            // Get stored refresh token
            var storedTokenValue = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            if (string.IsNullOrEmpty(storedTokenValue))
            {
                return new AuthResponseDto
                {
                    Errors = new() { "No refresh token found" },
                    Message = "Refresh token validation failed",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            var storedToken = JsonSerializer.Deserialize<RefreshTokenDto>(storedTokenValue);

            // Validate token
            if (storedToken == null)
            {
                return new AuthResponseDto
                {
                    Errors = new() { "Stored refresh token is corrupted" },
                    Message = "Refresh token validation failed",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            if (storedToken.RefreshToken != refreshToken)
            {
                return new AuthResponseDto
                {
                    Errors = new() { "Refresh token mismatch" },
                    Message = "Refresh token validation failed",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            if (storedToken.IsExpired)
            {
                return new AuthResponseDto
                {
                    Errors = new() { "Refresh token has expired" },
                    Message = "Refresh token validation failed",
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
                CreatedByIp = ipAddress,
            };

            // Mark old token revoked
            //storedToken.Revoked = DateTime.UtcNow;
            //storedToken.RevokedByIp = ipAddress;
            //storedToken.ReplacedByToken = newRefreshToken.RefreshToken;

            // Save new token
            await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", JsonSerializer.Serialize(newRefreshToken));

            return new AuthResponseDto
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
        }


        public async Task<bool> RevokeRefreshTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;
            await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            return true;
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


