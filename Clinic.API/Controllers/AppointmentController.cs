using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.AppointmentDtos;
using Clinic.API.BL.Interfaces.AppointmentInterfaces;
using Clinic.API.DL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _service;
        private readonly IMemoryCache _cache;

        public AppointmentsController(IAppointmentService service, IMemoryCache cache)
        {
            _service = service;
            _cache = cache;
        }

        private List<string> GetModelErrors() => ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CreateAppointmentDto>.Failure("Failed to create appointment", GetModelErrors()));

            var response = await _service.CreateAsync(dto);
            _cache.Remove("all_appointments");

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UpdateAppointmentDto>.Failure("Failed to update appointment", GetModelErrors()));

            var response = await _service.UpdateAsync(id, dto);
            _cache.Remove("all_appointments");

            return StatusCode(response.StatusCode, response);

        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _service.DeleteAsync(id);
            _cache.Remove("all_appointments");

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
            const string cacheKey = "all_appointments";

            if (_cache.TryGetValue<IReadOnlyList<AppointmentDto>>(cacheKey, out var cachedAppointments) && cachedAppointments != null)
                return Ok(ApiResponse<IReadOnlyList<AppointmentDto>>.Success(cachedAppointments, "Appointments retrieved successfully (from cache)"));

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
