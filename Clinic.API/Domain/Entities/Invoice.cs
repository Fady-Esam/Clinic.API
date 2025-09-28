using Clinic.API.Domain.Enums;

namespace Clinic.API.Domain.Entities
{
    public class Invoice 
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public Guid AppointmentId { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public InvoiceStatus Status { get; set; }

        // Navigation Properties
        public  Patient Patient { get; set; } = null!;
        public  Appointment Appointment { get; set; } = null!;
        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }
}
