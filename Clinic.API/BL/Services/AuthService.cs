using AutoMapper;
using Azure;
using Azure.Core;
using Clinic.API.BL.Dtos.AuthDtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.BL.Interfaces.AuthInterfaces;
using Clinic.API.BL.Interfaces.DoctorInterfaces;
using Clinic.API.DL.Models;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager, JWT jwt)
        {
            _userManager = userManager;
            _jwt = jwt;
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto, string? ipAddress)
        {
            const string errMessage = "Registration failed";
            if (await _userManager.Users.AsNoTracking().AnyAsync(u => u.NormalizedUserName == dto.UserName.ToUpper()))
                return ApiResponse<AuthResponseDto>.Failure(errMessage, new () { "User name is already in use by another user" });

            if (!string.IsNullOrWhiteSpace(dto.Email) && await _userManager.Users.AsNoTracking().AnyAsync(u => u.NormalizedEmail == dto.Email.ToUpper()))
                return ApiResponse<AuthResponseDto>.Failure(errMessage, new () { "Email is already in use by another user" });

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == dto.PhoneNumber))
                return ApiResponse<AuthResponseDto>.Failure(errMessage, new () { "Phone number is already in use by another user " });

            var invalidRoles = dto.Roles!.Where(r => !Enum.TryParse<UserRole>(r, ignoreCase: true, out _)).ToList();
            if (invalidRoles.Any())
                return ApiResponse<AuthResponseDto>.Failure(errMessage, invalidRoles.Select(r => $"Role '{r}' does not exist in the system.").ToList());

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return ApiResponse<AuthResponseDto>.Failure(errMessage, result.Errors.Select(e => e.Description).ToList());

            await _userManager.AddToRolesAsync(user, dto.Roles!);
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var accessToken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken(user.Id, ipAddress);
            await _userManager.SetAuthenticationTokenAsync(user, "ClinicApp", "RefreshToken", JsonSerializer.Serialize(newRefreshToken));

            var authResponseDtoData = new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = newRefreshToken.RefreshToken,
                Roles = roles,
                AccessTokenExpiration = accessToken.ValidTo,
                RefreshTokenExpiration = newRefreshToken.Expires
            };

            return ApiResponse<AuthResponseDto>.Success(authResponseDtoData, "User registered successfully", StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto, string? ipAddress)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return ApiResponse<AuthResponseDto>.Failure("Login failed", new () { "Invalid username or password" }, StatusCodes.Status401Unauthorized);

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var accessToken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken(user.Id, ipAddress);
            await _userManager.SetAuthenticationTokenAsync(user, "ClinicApp", "RefreshToken", JsonSerializer.Serialize(newRefreshToken));

            var authResponseDtoData = new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = newRefreshToken.RefreshToken,
                Roles = roles,
                AccessTokenExpiration = accessToken.ValidTo,
                RefreshTokenExpiration = newRefreshToken.Expires,
            };

            return ApiResponse<AuthResponseDto>.Success(authResponseDtoData, "Login successful");
        }

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(CreateRefreshTokenDto dto, string? createdByIp)
        {
            const string errMessage = "Getting new refresh token failed";
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return ApiResponse<AuthResponseDto>.Failure(errMessage, new () { "User not found" }, StatusCodes.Status404NotFound);

            var storedTokenValue = await _userManager.GetAuthenticationTokenAsync(user, "ClinicApp", "RefreshToken");
            if (string.IsNullOrEmpty(storedTokenValue)) return ApiResponse<AuthResponseDto>.Failure(errMessage, new () { "No refresh token found" }, StatusCodes.Status404NotFound);

            var storedToken = JsonSerializer.Deserialize<RefreshTokenDto>(storedTokenValue);
            if (storedToken == null) return ApiResponse<AuthResponseDto>.Failure(errMessage, new () { "Stored refresh token not found" }, StatusCodes.Status404NotFound);
            if (storedToken.RefreshToken != dto.RefreshToken) return ApiResponse<AuthResponseDto>.Failure(errMessage, new () { "Refresh token mismatch" });
            if (DateTime.UtcNow >= storedToken.Expires) return ApiResponse<AuthResponseDto>.Failure(errMessage, new () { "Refresh token has expired" }, StatusCodes.Status401Unauthorized);

            var newAccessToken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken(user.Id, createdByIp);
            await _userManager.SetAuthenticationTokenAsync(user, "ClinicApp", "RefreshToken", JsonSerializer.Serialize(newRefreshToken));

            var authResponseDto = new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken.RefreshToken,
                AccessTokenExpiration = newAccessToken.ValidTo,
                RefreshTokenExpiration = newRefreshToken.Expires,
            };

            return ApiResponse<AuthResponseDto>.Success(authResponseDto, "Token refreshed successfully");
        }

        public async Task<ApiResponse<bool>> RevokeRefreshTokenAsync(RevokeTokenDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return ApiResponse<bool>.Failure("Failed to revoke refresh token", new() { "User Not Found" }, StatusCodes.Status404NotFound);

            await _userManager.RemoveAuthenticationTokenAsync(user, "ClinicApp", "RefreshToken");
            return ApiResponse<bool>.Success(true, "Refresh Token Revoked Successfully");
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
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var item in roles)
                claims.Add(new Claim(ClaimTypes.Role, item));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credintial = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            return new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credintial
            );
        }

        private RefreshTokenDto GenerateRefreshToken(string userId, string? createdByIp)
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return new RefreshTokenDto
            {
                UserId = userId,
                RefreshToken = Convert.ToBase64String(randomBytes),
                CreatedByIp = createdByIp
            };
        }
    }

}


