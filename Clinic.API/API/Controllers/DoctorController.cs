using Clinic.API.API.Dtos.DoctorDtos;
using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.BL.Services;
using Clinic.API.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Clinic.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _service;
        private readonly IMemoryCache _cache;

        public DoctorsController(IDoctorService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDoctorDto dto)
        {
            var response = await _service.CreateAsync(dto);
            _cache.Remove("all_doctors");

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDoctorDto dto)
        {
            var response = await _service.UpdateAsync(id, dto);
            _cache.Remove("all_doctors");

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _service.DeleteAsync(id);
            _cache.Remove("all_doctors");

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            const string cacheKey = "all_doctors";

            if (_cache.TryGetValue<IReadOnlyList<DoctorDto>>(cacheKey, out var cachedDoctors) && cachedDoctors != null)
                return Ok(ApiResponse<IReadOnlyList<DoctorDto>>.Success(cachedDoctors, "Doctors retrieved successfully (from cache)"));

            var response = await _service.GetAllAsync();

            _cache.Set(cacheKey, response.Data, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(3),
                Priority = CacheItemPriority.Normal
            });

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] PagingDto dto)
        {
            var response = await _service.GetPagedAsync(dto);
            return StatusCode(response.StatusCode, response);
        }
    }

}
