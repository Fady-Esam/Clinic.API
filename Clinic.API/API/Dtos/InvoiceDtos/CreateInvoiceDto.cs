namespace Clinic.API.API.Dtos.InvoiceDtos
{
    public class CreateInvoiceDto
    {
        public Guid? PatientId { get; set; }
        public Guid? AppointmentId { get; set; }
        //public DateTime IssueDate { get; set; } = DateTime.UtcNow;
        //public DateTime DueDate { get; set; } = DateTime.UtcNow.AddDays(30);
        public List<CreateInvoiceItemDto> Items { get; set; } = new();
    }

}
