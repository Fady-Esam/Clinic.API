using Clinic.API.API.Dtos.MedicalRecordDtos;
using Clinic.API.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        [HttpGet("{id:guid}", Name = "GetMedicalRecordById")]
        //[Authorize(Roles = "Doctor,Patient")] // A Patient should only be able to see their own records (enforced in service)
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _medicalRecordService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("patient/{patientId:guid}")]
        //[Authorize(Roles = "Doctor,Patient")]
        public async Task<IActionResult> GetForPatient(Guid patientId)
        {
            // Advanced security in the service would check if the logged-in patient's ID matches patientId.
            var response = await _medicalRecordService.GetForPatientAsync(patientId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("doctor/{doctorId:guid}")]
        //[Authorize(Roles = "Doctor")] // Only a doctor can see a list of records they created.
        public async Task<IActionResult> GetForDoctor(Guid doctorId)
        {
            var response = await _medicalRecordService.GetForDoctorAsync(doctorId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
       // [Authorize(Roles = "Doctor")] // Only Doctors can create medical records.
        public async Task<IActionResult> Create([FromBody] CreateMedicalRecordDto dto)
        {
            var response = await _medicalRecordService.CreateAsync(dto);
            if (response.IsSuccess)
            {
                // On successful creation, return a 201 Created status with a link to the new resource.
                return CreatedAtRoute("GetMedicalRecordById", new { id = response.Data }, response);
            }
            return StatusCode(response.StatusCode, response);

        }

        [HttpPut("{id:guid}")]
       // [Authorize(Roles = "Doctor")] // Only Doctors can update records.
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMedicalRecordDto dto)
        {
            var response = await _medicalRecordService.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);

        }

        [HttpDelete("{id:guid}")]
        //[Authorize(Roles = "Doctor")] // Only Doctors can delete records.
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _medicalRecordService.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }
    }
}
