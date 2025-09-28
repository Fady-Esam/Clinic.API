using Clinic.API.API.Dtos.UserClaimDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Clinic.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class UserClaimsController : ControllerBase
    {
        private readonly IUserClaimService _service;
        private readonly IMemoryCache _cache;

        public UserClaimsController(IUserClaimService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> AddClaim([FromBody] CreateUserClaimDto dto)
        {
            var response = await _service.AddAsync(dto);
            _cache.Remove("all_user_claims");

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateClaim(int id, [FromBody] UpdateUserClaimDto dto)
        {
            var response = await _service.UpdateAsync(id, dto);
            _cache.Remove("all_user_claims");

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteClaim(int id)
        {
            var response = await _service.DeleteAsync(id);
            _cache.Remove("all_user_claims");

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("by-id/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
           
            var response = await _service.GetByUserIdAsync(userId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            const string cacheKey = "all_user_claims";

            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<IdentityUserClaim<string>>? cachedClaims) && cachedClaims != null)
            {
                return Ok(ApiResponse<IReadOnlyList<IdentityUserClaim<string>>>.Success(
                    cachedClaims,
                    "User Claims retrieved successfully (from cache)"
                ));
            }

            var response = await _service.GetAllAsync();

            if (response.Data != null)
            {
                _cache.Set(cacheKey, response.Data, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),
                    Priority = CacheItemPriority.Normal
                });
            }

            return StatusCode(response.StatusCode, response);
        }
    }

}
