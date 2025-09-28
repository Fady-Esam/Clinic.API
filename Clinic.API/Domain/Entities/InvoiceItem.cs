using System.ComponentModel.DataAnnotations;

namespace Clinic.API.Domain.Entities
{
    public class InvoiceItem 
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(200)]
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        // Navigation Property
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;
    }
}
