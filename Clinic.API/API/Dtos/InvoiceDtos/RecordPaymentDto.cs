namespace Clinic.API.API.Dtos.InvoiceDtos
{
    public class RecordPaymentDto
    {
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }
}
