
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.DoctorDtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.BL.Interfaces.DoctorInterfaces;
using Clinic.API.BL.Interfaces.PatientInterfaces;
using Clinic.API.BL.Services;
using Clinic.API.DL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class DoctorsController : Controller
    {
        private readonly IDoctorService _service;
        private readonly IMemoryCache _cache;

        public DoctorsController(IDoctorService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }
        private List<string> GetModelErrors()
        {
            return ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrUpdateDoctorDto dto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(new ApiResponse<CreateOrUpdateDoctorDto>
                {
                    Message = "Failed to create doctor",
                    Errors = GetModelErrors(),
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var response = await _service.CreateAsync(dto);
            _cache.Remove("all_doctors");

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrUpdateDoctorDto dto)
        {
            if (!ModelState.IsValid)
            {

                return BadRequest(new ApiResponse<CreateOrUpdateDoctorDto>
                {
                    Message = "Failed to update doctor",
                    Errors = GetModelErrors(),
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
            dto.Id = id;
   
            var response = await _service.UpdateAsync(dto);
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
            {
                return Ok(new ApiResponse<IReadOnlyList<DoctorDto>>
                {
                    Data = cachedDoctors,
                    Message = "Doctors retrieved successfully (from cache)",
                    StatusCode = StatusCodes.Status200OK
                });
            }

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
