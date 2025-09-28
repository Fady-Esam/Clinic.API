using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.API.Dtos.PatientDtos;
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
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _service;
        private readonly IMemoryCache _cache;

        public PatientsController(IPatientService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePatientDto dto)
        {

            
            var response = await _service.CreateAsync(dto);
            _cache.Remove("all_patients");
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePatientDto dto)
        {
            var response = await _service.UpdateAsync(id, dto);
            _cache.Remove("all_patients");
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _service.DeleteAsync(id);
            _cache.Remove("all_patients");
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
            const string cacheKey = "all_patients";

            if (_cache.TryGetValue<IReadOnlyList<PatientDto>>(cacheKey, out var cachedPatients) && cachedPatients != null)
                return Ok(ApiResponse<IReadOnlyList<PatientDto>>.Success(cachedPatients, "Patients retrieved successfully (from cache)"));

            var response = await _service.GetAllAsync();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(3),
                Priority = CacheItemPriority.Normal
            };

            _cache.Set(cacheKey, response.Data, cacheOptions);

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


