using Clinic.API.API.Dtos.PatientDtos;
using Clinic.API.Domain.Enums;

namespace Clinic.API.API.Dtos.InvoiceDtos
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public PatientDto Patient { get; set; } = null!;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue => TotalAmount - AmountPaid;
        public string Status { get; set; } = string.Empty;
        public List<InvoiceItemDto> Items { get; set; } = new ();
    }
}
