using Clinic.API.Domain.Entities;

namespace Clinic.API.Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        // Create
        Task AddAsync(Invoice invoice);

        // Read
        Task<Invoice?> GetByIdAsync(Guid id);
        Task<Invoice?> GetByIdWithDetailsAsync(Guid id);
        Task<IReadOnlyList<Invoice>> GetByPatientIdAsync(Guid patientId);
        Task<string> GetNextInvoiceNumberAsync();

        // Update
        Task UpdateAsync(Invoice invoice);

        // Delete
        Task DeleteAsync(Invoice invoice);
    }
}
