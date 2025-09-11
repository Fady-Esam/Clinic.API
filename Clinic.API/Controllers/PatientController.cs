using Clinic.API.BL.Dtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.DL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _service;

        public PatientsController(IPatientService service)
        {
            _service = service;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var patient = await _service.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound(new ApiResponse<PatientDto>
                {
                    Message = "Patient not found",
                    Errors = new () { "No patient exists with the given id" },
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            return Ok(new ApiResponse<PatientDto>
            {
                Data = patient,
                Message = "Patient retrieved successfully",
                StatusCode = StatusCodes.Status200OK
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _service.GetAllAsync();

            return Ok(new ApiResponse<IReadOnlyList<PatientDto>>
            {
                Data = patients,
                Message = "Patients retrieved successfully",
                StatusCode = StatusCodes.Status200OK
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(PatientDto dto)
        {

            if (!ModelState.IsValid)
            {
                // Model validation errors
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<PatientDto>
                {
                    Message = "Validation error",
                    Errors = errors,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, new ApiResponse<PatientDto>
                {
                    Data = created,
                    Message = "Patient created successfully",
                    StatusCode = StatusCodes.Status201Created
                });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, PatientDto dto)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<PatientDto>
                {
                    Data = null!,
                    Message = "Validation error",
                    Errors = errors,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }


            if (id != dto.Id)
            {
                return BadRequest(new ApiResponse<PatientDto>
                {
                    Message = "Id mismatch",
                    Errors = new () { "Route id does not match patient id" },
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }

            var updated = await _service.UpdateAsync(dto);
            if (updated == null)
            {
                return NotFound(new ApiResponse<PatientDto>
                {
                    Message = "Patient not found",
                    Errors = new () { "No patient exists with the given id" },
                    StatusCode = StatusCodes.Status404NotFound
                });
            }

            return Ok(new ApiResponse<PatientDto>
            {
                Data = updated,
                Message = "Patient updated successfully",
                StatusCode = StatusCodes.Status200OK

            });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            var result = await _service.DeleteAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponse<PatientDto>
                {
                    Message = "Patient not found",
                    Errors = new () { "No patient exists with the given id" },
                    StatusCode = StatusCodes.Status404NotFound

                });
            }

            return Ok(new ApiResponse<PatientDto>
            {
                Data = dto!,
                Message = "Patient deleted successfully",
                StatusCode = StatusCodes.Status200OK
            });
        }
    }
}
