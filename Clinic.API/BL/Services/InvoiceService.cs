using AutoMapper;
using Clinic.API.API.Dtos.InvoiceDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;
using Clinic.API.Domain.Interfaces;

namespace Clinic.API.BL.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public InvoiceService(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<Guid>> CreateAsync(CreateInvoiceDto dto)
        {
            var invoiceEntity = _mapper.Map<Invoice>(dto);

            invoiceEntity.InvoiceNumber = await _invoiceRepository.GetNextInvoiceNumberAsync();
            invoiceEntity.TotalAmount = invoiceEntity.Items.Sum(item => item.TotalPrice);

            await _invoiceRepository.AddAsync(invoiceEntity);

            return ApiResponse<Guid>.Success(invoiceEntity.Id, "Invoice created successfully.", 201);
        }

        public async Task<ApiResponse<object>> RecordPaymentAsync(Guid invoiceId, RecordPaymentDto dto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
                return ApiResponse<object>.Failure("Not Found", new() { "Invoice not found." }, 404);

            if (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.Void)
                return ApiResponse<object>.Failure("Invalid Operation", new() { "This invoice is already paid or voided." }, 400);

            invoice.AmountPaid += dto.Amount;
            invoice.Status = (invoice.AmountPaid >= invoice.TotalAmount) ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;

            await _invoiceRepository.UpdateAsync(invoice);

            return ApiResponse<object>.SuccessNoData("Payment recorded successfully.");
        }

        public async Task<ApiResponse<object>> VoidAsync(Guid invoiceId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
            if (invoice == null)
                return ApiResponse<object>.Failure("Not Found", new() { "Invoice not found." }, 404);

            invoice.Status = InvoiceStatus.Void;
            await _invoiceRepository.UpdateAsync(invoice);

            return ApiResponse<object>.SuccessNoData("Invoice voided successfully.");
        }

        public async Task<ApiResponse<InvoiceDto>> GetByIdAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(id);
            if (invoice == null)
                return ApiResponse<InvoiceDto>.Failure("Not Found", new() { "Invoice not found." }, 404);

            var invoiceDto = _mapper.Map<InvoiceDto>(invoice);
            return ApiResponse<InvoiceDto>.Success(invoiceDto, "Invoice retrieved successfully.");
        }

        public async Task<ApiResponse<IReadOnlyList<InvoiceDto>>> GetForPatientAsync(Guid patientId)
        {
            var invoices = await _invoiceRepository.GetByPatientIdAsync(patientId);
            var invoiceDtos = _mapper.Map<IReadOnlyList<InvoiceDto>>(invoices);
            return ApiResponse<IReadOnlyList<InvoiceDto>>.Success(invoiceDtos, "Invoices for patient retrieved successfully.");
        }
    }
}
