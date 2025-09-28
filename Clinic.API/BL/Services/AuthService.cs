using AutoMapper;
using Azure;
using Azure.Core;
using Clinic.API.API.Dtos.AuthDtos;
using Clinic.API.API.Dtos.AuthDtos.FacebookDtos;
using Clinic.API.API.Dtos.AuthDtos.PasswordDtos;
using Clinic.API.API.Dtos.AuthDtos.RefreshTokenDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Exceptions;
using Clinic.API.Common.Responses;
using Clinic.API.Common.Settings;
using Clinic.API.DL;
using Clinic.API.DL.Repositories;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;
using Clinic.API.Domain.Interfaces;
using Google.Apis.Auth;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
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
//using Twilio.Http;

namespace Clinic.API.BL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ISmsService _SmsService;
        private readonly IUserConfirmationCodeService _confirmationCodeService;
        private readonly IRefreshTokenRepository _refreshTokenRepo;
        private readonly JwtSetting _jwtSetting;
        private readonly GoogleSetting _googleSetting;
        private readonly FacebookSetting _facebookSetting;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMapper mapper, IEmailService emailService, ISmsService SmsService, IUserConfirmationCodeService confirmationCodeService, IRefreshTokenRepository refreshTokenRepo, JwtSetting jwtSetting, GoogleSetting googleSetting, FacebookSetting facebookSetting)
        {
            _userManager = userManager;
            _emailService = emailService;
            _SmsService = SmsService;
            _confirmationCodeService = confirmationCodeService;
            _jwtSetting = jwtSetting;
            _googleSetting = googleSetting;
            _facebookSetting = facebookSetting;
            _refreshTokenRepo = refreshTokenRepo;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            const string errMessage = "Registration failed";

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrWhiteSpace(dto.UserName) && await _userManager.Users.AsNoTracking().AnyAsync(u => u.NormalizedUserName == dto.UserName.ToUpper()))
                    return ApiResponse<AuthResponseDto>.Failure(errMessage, new() { "User name is already in use by another user" });

                if (!string.IsNullOrWhiteSpace(dto.Email) && await _userManager.Users.AsNoTracking().AnyAsync(u => u.NormalizedEmail == dto.Email.ToUpper()))
                    return ApiResponse<AuthResponseDto>.Failure(errMessage, new() { "Email is already in use by another user" });

                if (!string.IsNullOrWhiteSpace(dto.PhoneNumber) && await _userManager.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == dto.PhoneNumber))
                    return ApiResponse<AuthResponseDto>.Failure(errMessage, new() { "Phone number is already in use by another user " });


                var user = new ApplicationUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                    return ApiResponse<AuthResponseDto>.Failure(errMessage, result.Errors.Select(e => e.Description).ToList());

                if (!string.IsNullOrEmpty(user.Email))
                    await _emailService.SendConfirmationCodeAsync(user.Email, await _confirmationCodeService.GenerateCodeAsync(user.Id, ConfirmationType.Email));

                if (!string.IsNullOrEmpty(user.PhoneNumber))
                    await _SmsService.SendConfirmationCodeAsync(user.PhoneNumber, await _confirmationCodeService.GenerateCodeAsync(user.Id, ConfirmationType.SMS));

                var accessToken = await GenerateJwtToken(user);

                var createDto = new CreateNewRefreshTokenDto { UserId = user.Id };
                var newRefreshToken = _mapper.Map<RefreshToken>(createDto);
                await _refreshTokenRepo.AddAsync(newRefreshToken);

                var authResponseDtoData = new AuthResponseDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                    RefreshToken = newRefreshToken.Token,
                    Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                    Claims = accessToken.Claims.ToList(),
                    AccessTokenExpiration = accessToken.ValidTo,
                    RefreshTokenExpiration = newRefreshToken.Expires
                };

                return ApiResponse<AuthResponseDto>.Success(authResponseDtoData, "User registered successfully", StatusCodes.Status201Created);
            }catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new ApiException(errMessage, new() { ex.Message });
            }
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            const string errMessage = "Login failed";

            ApplicationUser? user = null;

            if (!string.IsNullOrWhiteSpace(dto.UserName))
                user = await _userManager.FindByNameAsync(dto.UserName);

            if (user == null && !string.IsNullOrWhiteSpace(dto.Email))
            {
                user = await _userManager.FindByEmailAsync(dto.Email);
                if(user != null && !user.EmailConfirmed)
                    return ApiResponse<AuthResponseDto>.Failure(
                            errMessage,
                            new () { "Email is not confirmed" },
                            StatusCodes.Status403Forbidden
                        );
            }

            if (user == null && !string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

                if (user != null && !user.PhoneNumberConfirmed)
                    return ApiResponse<AuthResponseDto>.Failure(
                            errMessage,
                            new () { "Phone Number is not confirmed" },
                            StatusCodes.Status403Forbidden
                        );
            }

            // If user not found or password does not match
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return ApiResponse<AuthResponseDto>.Failure(
                    errMessage,
                    new () { "Invalid credentials" },
                    StatusCodes.Status401Unauthorized
                );
            }

            var accessToken = await GenerateJwtToken(user);
            var createDto = new CreateNewRefreshTokenDto { UserId = user.Id };
            var newRefreshToken = _mapper.Map<RefreshToken>(createDto);
            await _refreshTokenRepo.AddAsync(newRefreshToken);
            var authResponseDtoData = new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = newRefreshToken.Token,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                Claims = accessToken.Claims.ToList(),
                AccessTokenExpiration = accessToken.ValidTo,
                RefreshTokenExpiration = newRefreshToken.Expires,
            };

            return ApiResponse<AuthResponseDto>.Success(authResponseDtoData, "Login successful");
        }


        public async Task<ApiResponse<AuthResponseDto>> GoogleLoginAsync(string idToken)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _googleSetting.ClientId }
                });
            }
            catch
            {
                return ApiResponse<AuthResponseDto>.Failure("Failed to login with google", new() { "Invalid google token" });
            }

            // Check if user exists by Facebook Id (ProviderKey)
            ApplicationUser? userLogin = null;

            foreach (var us in _userManager.Users)
            {
                var logins = await _userManager.GetLoginsAsync(us);
                if (logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == payload.Subject))
                {
                    userLogin = us;
                    break;
                }
            }

            ApplicationUser? user = null;
            if (userLogin == null)
            {
                // Create user
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = payload.Name,
                        Email = payload.Email,
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return ApiResponse<AuthResponseDto>.Failure("Failed to create user", result.Errors.Select(e => e.Description).ToList());
                }

                // Add login provider
                var loginInfo = new UserLoginInfo("Google", payload.Subject, "Google");
                var loginResult = await _userManager.AddLoginAsync(user, loginInfo);
                if (!loginResult.Succeeded)
                    return ApiResponse<AuthResponseDto>.Failure("Failed to add Google login", loginResult.Errors.Select(e => e.Description).ToList());
            }
            else
            {
                user = userLogin;
            }

            await _userManager.SetAuthenticationTokenAsync(user, "Google", "id_token", idToken);

            var accessToken = await GenerateJwtToken(user);
            var newRefreshTokenGenerated = GenerateRefreshToken();
            var createDto = new CreateNewRefreshTokenDto { UserId = user.Id };
            var newRefreshToken = _mapper.Map<RefreshToken>(createDto);
            await _refreshTokenRepo.AddAsync(newRefreshToken);
            var authResponseDtoData = new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = newRefreshToken.Token,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                Claims = accessToken.Claims.ToList(),
                //Roles = userLogin != null ? (await _userManager.GetRolesAsync(user)).ToList() : new(),
                //Claims = userLogin != null ? accessToken.Claims.ToList() : new(),
                AccessTokenExpiration = accessToken.ValidTo,
                RefreshTokenExpiration = newRefreshToken.Expires
            };
            return ApiResponse<AuthResponseDto>.Success(authResponseDtoData, "Login successful");

        }

        public async Task<ApiResponse<AuthResponseDto>> FacebookLoginAsync(string accessToken)
        {
            const string errMessage = "Failed to login with facebook";
            string appId = _facebookSetting.AppId;
            string appSecret = _facebookSetting.AppSecret;
            var appAccessToken = $"{appId}|{appSecret}";

            using var httpClient = new HttpClient();

            var debugTokenResponse = await httpClient.GetFromJsonAsync<FacebookDebugTokenResponseDto>(
                $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appAccessToken}"
            );

            if (debugTokenResponse?.Data?.IsValid != true)
                return ApiResponse<AuthResponseDto>.Failure(errMessage, new() { "Invalid Facebook token" });

            var response = await httpClient.GetFromJsonAsync<FacebookUserResponseDto>(
                $"https://graph.facebook.com/me?fields=id,email,name&access_token={accessToken}");

            if (response == null || string.IsNullOrEmpty(response.Id))
                return ApiResponse<AuthResponseDto>.Failure(errMessage, new() { "Invalid Facebook token" });

            ApplicationUser? userLogin = null;

            foreach (var us in _userManager.Users)
            {
                var logins = await _userManager.GetLoginsAsync(us);
                if (logins.Any(l => l.LoginProvider == "Facebook" && l.ProviderKey == response.Id))
                {
                    userLogin = us;
                    break;
                }
            }

            ApplicationUser? user = null;
            if (userLogin == null)
            {
                user = await _userManager.FindByEmailAsync(response.Email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = response.Name,
                        Email = response.Email,
                        EmailConfirmed = true
                    };
                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return ApiResponse<AuthResponseDto>.Failure("Failed to create user", result.Errors.Select(e => e.Description).ToList());
                }

                var loginInfo = new UserLoginInfo("Facebook", response.Id, "Facebook");
                var loginResult = await _userManager.AddLoginAsync(user, loginInfo);
                if (!loginResult.Succeeded)
                    return ApiResponse<AuthResponseDto>.Failure("Failed to add Facebook login", loginResult.Errors.Select(e => e.Description).ToList());
            }
            else
            {
                user = userLogin;
            }

            await _userManager.SetAuthenticationTokenAsync(user, "Facebook", "access_token", accessToken);
            var jwtAccessToken = await GenerateJwtToken(user);
            var newRefreshTokenGenerated = GenerateRefreshToken();
            var createDto = new CreateNewRefreshTokenDto { UserId = user.Id };
            var newRefreshToken = _mapper.Map<RefreshToken>(createDto);
            await _refreshTokenRepo.AddAsync(newRefreshToken);
            var authResponseDtoData = new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtAccessToken),
                RefreshToken = newRefreshToken.Token,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                Claims = jwtAccessToken.Claims.ToList(),
                //Roles = userLogin != null ? (await _userManager.GetRolesAsync(user)).ToList() : new(),
                //Claims = userLogin != null ? jwtAccessToken.Claims.ToList() : new(),
                AccessTokenExpiration = jwtAccessToken.ValidTo,
                RefreshTokenExpiration = newRefreshToken.Expires
            };
            return ApiResponse<AuthResponseDto>.Success(authResponseDtoData, "Login successful");
        }


        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(CreateNewRefreshTokenDto dto)
        {
            const string errMessage = "Token refresh failed";

            var storedToken = await _refreshTokenRepo.GetByTokenAsync(dto.Token);
            if (storedToken == null)
                return ApiResponse<AuthResponseDto>.Failure(errMessage, new() { "Refresh token not found" }, StatusCodes.Status404NotFound);

            // var tokenDto = _mapper.Map<RefreshTokenDto>(storedToken);

            if (!storedToken.IsActive)
            {
                if (storedToken.ReplacedByToken != null)
                {
                    return ApiResponse<AuthResponseDto>.Failure(
                        "Token reuse detected. Please login again.",
                        new() { "This refresh token has already been rotated" },
                        StatusCodes.Status401Unauthorized
                    );
                }

                return ApiResponse<AuthResponseDto>.Failure(errMessage, new() { "Refresh token is not active" }, StatusCodes.Status401Unauthorized);
            }

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
                return ApiResponse<AuthResponseDto>.Failure(errMessage, new() { "User not found" }, StatusCodes.Status404NotFound);

            var newRefreshTokenGenerated = GenerateRefreshToken();

            storedToken.Revoked = DateTime.UtcNow;
            storedToken.ReplacedByToken = newRefreshTokenGenerated;
            _refreshTokenRepo.Update(storedToken);
            dto.UserId = user.Id; 
            var newRefreshToken = _mapper.Map<RefreshToken>(dto);
            newRefreshToken.Token = newRefreshTokenGenerated;
            await _refreshTokenRepo.AddAsync(newRefreshToken);

            // Generate new JWT
            var newJwt = await GenerateJwtToken(user);

            var response = new AuthResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newJwt),
                RefreshToken = newRefreshToken.Token,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                Claims = newJwt.Claims.ToList(),
                AccessTokenExpiration = newJwt.ValidTo,
                RefreshTokenExpiration = newRefreshToken.Expires
            };

            return ApiResponse<AuthResponseDto>.Success(response, "Token refreshed successfully");
        }

        public async Task<ApiResponse<object>> RevokeRefreshTokenAsync(RevokeRefreshTokenDto dto)
        {
            var storedToken = await _refreshTokenRepo.GetByTokenAsync(dto.Token);
            if (storedToken == null)
                return ApiResponse<object>.Failure("Failed to revoke refresh token", new() { "Token not found" }, StatusCodes.Status404NotFound);

            var user = await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
                return ApiResponse<object>.Failure(
                    "Failed to revoke refresh token",
                    new() { "User not found" },
                    StatusCodes.Status404NotFound);

            //storedToken.Revoked = DateTime.UtcNow;
            _mapper.Map(dto, storedToken);
            _refreshTokenRepo.Update(storedToken);

            var googleIdToken = await _userManager.GetAuthenticationTokenAsync(user, "Google", "id_token");
            if (!string.IsNullOrEmpty(googleIdToken))
                await _userManager.RemoveAuthenticationTokenAsync(user, "Google", "id_token");

            var facebookAccessToken = await _userManager.GetAuthenticationTokenAsync(user, "Facebook", "access_token");
            if (!string.IsNullOrEmpty(facebookAccessToken))
                await _userManager.RemoveAuthenticationTokenAsync(user, "Facebook", "access_token");


            return ApiResponse<object>.SuccessNoData("Refresh Token Revoked Successfully");
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        
        // Change Password
        public async Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordDto dto)
        {

            const string errMessage = "Failed to change password";
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return ApiResponse<object>.Failure(errMessage, new() { "User not found" }, StatusCodes.Status404NotFound);

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<object>.Failure(errMessage, errors);
            }

            return ApiResponse<object>.SuccessNoData("Password changed successfully");
        }

        // Request Password Reset
        public async Task<ApiResponse<object>> RequestPasswordResetAsync(PasswordResetRequestDto dto)
        {
            const string errMessage = "Failed to reset password";
            ApplicationUser? user = null;
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    return ApiResponse<object>.Failure(
                            errMessage,
                            new () { "Email not registered" },
                            StatusCodes.Status404NotFound
                        );
                var code = await _confirmationCodeService.GenerateCodeAsync(user.Id, ConfirmationType.PasswordReset);
                await _emailService.SendPasswordResetCodeAsync(user.Email!, code);
                return ApiResponse<object>.SuccessNoData("Password reset code sent to email");
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

                if (user == null)
                    return ApiResponse<object>.Failure(
                            errMessage,
                            new () { "Phone Number is not registered" },
                            StatusCodes.Status404NotFound
                        );
                var code = await _confirmationCodeService.GenerateCodeAsync(user.Id, ConfirmationType.PasswordReset);
                await _SmsService.SendPasswordResetCodeAsync(user.PhoneNumber!, code);
                return ApiResponse<object>.SuccessNoData("Password reset code sent to phone number");

            }
            return ApiResponse<object>.Failure(errMessage, new () { "User not found" }, StatusCodes.Status404NotFound);
        }




        // Confirm Password Reset
        public async Task<ApiResponse<object>> ConfirmPasswordResetAsync(PasswordResetConfirmDto dto)
        {

            const string errMessage = "Failed to confirm reset password";
            ApplicationUser? user = null;
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                    return ApiResponse<object>.Failure(
                            errMessage,
                            new() { "Email not registered" },
                            StatusCodes.Status404NotFound
                        );
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            {
                user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);

                if (user == null)
                    return ApiResponse<object>.Failure(
                            errMessage,
                            new() { "Phone Number is not registered" },
                            StatusCodes.Status404NotFound
                        );

            }
            if (user == null)
                return ApiResponse<object>.Failure(errMessage, new() { "User not found" }, StatusCodes.Status404NotFound);

            var isValid = await _confirmationCodeService.ValidateCodeAsync(user.Id, dto.Code, ConfirmationType.PasswordReset);
            if (!isValid)
                return ApiResponse<object>.Failure(errMessage, new() { "Invalid or expired reset code" }, StatusCodes.Status400BadRequest);

            var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<object>.Failure(errMessage, errors);
            }

            return ApiResponse<object>.SuccessNoData("Password reset successfully");

        }


        public async Task<ApiResponse<object>> ConfirmEmailAsync(EmailConfirmDto dto)
        {
            const string errMessage = "Failed to confirm Email";

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return ApiResponse<object>.Failure(errMessage, new() { "Email not registered" }, StatusCodes.Status404NotFound);

            // Retrieve code from cache (or db)
            var isValid = await _confirmationCodeService.ValidateCodeAsync(user.Id, dto.Code, ConfirmationType.Email);
            if (!isValid)
                return ApiResponse<object>.Failure(errMessage, new() { "Invalid Confirmation Code" });

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return ApiResponse<object>.SuccessNoData("Email confirmed successfully");
        }

        public async Task<ApiResponse<object>> ConfirmSmsAsync(SmsConfirmDto dto)
        {
            const string errMessage = "Failed to confirm Phone Number";
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == dto.PhoneNumber);
            if (user == null)
                return ApiResponse<object>.Failure(errMessage, new() { "Phone number not registered" }, StatusCodes.Status404NotFound);

            var isValid = await _confirmationCodeService.ValidateCodeAsync(user.Id, dto.Code, ConfirmationType.SMS);
            if (!isValid)
                return ApiResponse<object>.Failure(errMessage, new() { "Invalid Confirmation Code" });


            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);

            return ApiResponse<object>.SuccessNoData("Phone number confirmed successfully");
        }




        // 🔹 Helpers
        private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // unique token id
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSetting.ExpirationInMinutes);

            return new JwtSecurityToken(
                issuer: _jwtSetting.Issuer,
                audience: _jwtSetting.Audience,
                claims: claims,
                expires: tokenExpiration,
                signingCredentials: credentials
            );
        }

       
    }


}


