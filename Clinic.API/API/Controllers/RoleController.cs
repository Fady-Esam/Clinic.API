using Clinic.API.API.Dtos.RoleDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Clinic.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _service;
        private readonly IMemoryCache _cache;

        public RolesController(IRoleService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
        {
            var response = await _service.CreateRoleAsync(dto);
            _cache.Remove("all_roles");

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateRoleDto dto)
        {
            var response = await _service.UpdateRoleAsync(id, dto);
            _cache.Remove("all_roles");
            return StatusCode(response.StatusCode, response);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _service.DeleteRoleAsync(id);
            _cache.Remove("all_roles");
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string cacheKey = "all_roles";

            if (_cache.TryGetValue<IReadOnlyList<IdentityRole>>(cacheKey, out var cachedRoles) && cachedRoles != null)
                return Ok(ApiResponse<IReadOnlyList<IdentityRole>>.Success(cachedRoles, "Roles retrieved successfully (from cache)"));

            var response = await _service.GetAllRolesAsync();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(3),
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(cacheKey, response.Data, cacheOptions);
            return StatusCode(response.StatusCode, response);
        }
    }

}