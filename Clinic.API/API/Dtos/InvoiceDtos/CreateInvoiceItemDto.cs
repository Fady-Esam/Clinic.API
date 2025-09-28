namespace Clinic.API.API.Dtos.InvoiceDtos
{
    public class CreateInvoiceItemDto
    {
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
