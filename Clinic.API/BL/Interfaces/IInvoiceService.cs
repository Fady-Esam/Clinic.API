using Clinic.API.API.Dtos.InvoiceDtos;
using Clinic.API.Common.Responses;

namespace Clinic.API.BL.Interfaces
{
    public interface IInvoiceService
    {
        Task<ApiResponse<InvoiceDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IReadOnlyList<InvoiceDto>>> GetForPatientAsync(Guid patientId);
        Task<ApiResponse<Guid>> CreateAsync(CreateInvoiceDto dto);
        Task<ApiResponse<object>> RecordPaymentAsync(Guid invoiceId, RecordPaymentDto dto);
        Task<ApiResponse<object>> VoidAsync(Guid invoiceId);
    }
}
