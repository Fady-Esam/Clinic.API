using Clinic.API.API.Dtos.RoleClaimDtos;
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
    public class RoleClaimsController : ControllerBase
    {
        private readonly IRoleClaimsService _service;
        private readonly IMemoryCache _cache;

        public RoleClaimsController(IRoleClaimsService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> AddRoleClaim([FromBody] CreateRoleClaimDto dto)
        {
            var response = await _service.AddRoleClaimAsync(dto);
            _cache.Remove("all_role_claims");
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoleClaim(int id, [FromBody] UpdateRoleClaimDto dto)
        {
            var response = await _service.UpdateRoleClaimAsync(id, dto);
            _cache.Remove("all_role_claims");
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleClaim(int id)
        {
            var response = await _service.DeleteRoleClaimAsync(id);
            _cache.Remove("all_role_claims");
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("by-role-name/{roleName}")]
        public async Task<IActionResult> GetByRoleName(string roleName)
        {
            var response = await _service.GetByRoleNameAsync(roleName);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("by-role-id/{roleId}")]
        public async Task<IActionResult> GetByRoleId(string roleId)
        {
            const string cacheKey = "all_role_claims";

            if (_cache.TryGetValue<IReadOnlyList<IdentityRoleClaim<string>>>(cacheKey, out var cachedClaims))
                return Ok(ApiResponse<IReadOnlyList<IdentityRoleClaim<string>>>.Success(cachedClaims, "Role claims retrieved from cache"));

            var response = await _service.GetByRoleIdAsync(roleId);

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(3),
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(cacheKey, response.Data, cacheOptions);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            const string cacheKey = "all_role_claims";

            if (_cache.TryGetValue<IReadOnlyList<IdentityRoleClaim<string>>>(cacheKey, out var cachedClaims))
                return Ok(ApiResponse<IReadOnlyList<IdentityRoleClaim<string>>>.Success(cachedClaims, "Role claims retrieved from cache"));

            var response = await _service.GetAllAsync();
            _cache.Set(cacheKey, response.Data, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(3)
            });

            return StatusCode(response.StatusCode, response);
        }
    }
}
