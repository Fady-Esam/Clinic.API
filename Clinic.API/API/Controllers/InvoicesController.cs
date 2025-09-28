using Clinic.API.API.Dtos.InvoiceDtos;
using Clinic.API.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        public InvoicesController(IInvoiceService invoiceService) { _invoiceService = invoiceService; }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInvoiceDto dto)
        {
            var response = await _invoiceService.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);

        }

        [HttpGet("{id:guid}", Name = "GetInvoiceById")]
        //[Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _invoiceService.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);

        }

        [HttpGet("patient/{patientId:guid}")]
        //[Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<IActionResult> GetForPatient(Guid patientId)
        {
            // Add security logic to ensure patient can only see their own invoices
            var response = await _invoiceService.GetForPatientAsync(patientId);
            return StatusCode(response.StatusCode, response);

        }

        [HttpPost("{id:guid}/payments")]
        public async Task<IActionResult> RecordPayment(Guid id, [FromBody] RecordPaymentDto dto)
        {
            var response = await _invoiceService.RecordPaymentAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Void(Guid id)
        {
            var response = await _invoiceService.VoidAsync(id);
            return StatusCode(response.StatusCode, response);

        }
    }
}
