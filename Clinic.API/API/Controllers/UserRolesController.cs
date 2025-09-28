using Clinic.API.API.Dtos.UserRoleDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Clinic.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]

    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleService _service;
        private readonly IMemoryCache _cache;


        public UserRolesController(IUserRoleService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> AddRolesToUsers([FromBody] CreateUserRoleDto dto)
        {
            var result = await _service.AddRolesToUsersAsync(dto);
            _cache.Remove("all_user_roles");
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRoleDto dto)
        {
            var result = await _service.UpdateUserRolesAsync(dto);
            _cache.Remove("all_user_roles");
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRolesFromUsers([FromBody] CreateUserRoleDto dto)
        {
            var result = await _service.RemoveRolesFromUsersAsync(dto);
            _cache.Remove("all_user_roles");
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var result = await _service.GetUserRolesAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserRoles()
        {
            const string cacheKey = "all_user_roles";

            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<IdentityUserRole<string>>? cachedRoles) && cachedRoles != null)
            {
                return Ok(ApiResponse<IReadOnlyList<IdentityUserRole<string>>>.Success(
                    cachedRoles,
                    "User Roles retrieved successfully (from cache)"
                ));
            }

            var result = await _service.GetAllUserRolesAsync();

            if (result.Data != null)
            {
                _cache.Set(cacheKey, result.Data, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(3),
                    Priority = CacheItemPriority.Normal
                });
            }

            return StatusCode(result.StatusCode, result);
        }

    }

}
