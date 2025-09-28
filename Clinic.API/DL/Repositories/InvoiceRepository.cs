using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.DL.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<Invoice?> GetByIdAsync(Guid id)
        {
            return await _context.Invoices.FindAsync(id);
        }

        public async Task<Invoice?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.Invoices
                .Include(i => i.Items)
                .Include(i => i.Patient)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IReadOnlyList<Invoice>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Invoices
                .Where(i => i.PatientId == patientId)
                .OrderByDescending(i => i.IssueDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<string> GetNextInvoiceNumberAsync()
        {
            var lastInvoice = await _context.Invoices
                .OrderByDescending(i => i.IssueDate)
                .FirstOrDefaultAsync();

            if (lastInvoice == null || lastInvoice.IssueDate.Year != DateTime.UtcNow.Year)
            {
                return $"INV-{DateTime.UtcNow.Year}-0001";
            }

            var lastNumber = int.Parse(lastInvoice.InvoiceNumber.Split('-').Last());
            return $"INV-{DateTime.UtcNow.Year}-{(lastNumber + 1):D4}";
        }

        public async Task UpdateAsync(Invoice invoice)
        {
             _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();


        }

        public async Task DeleteAsync(Invoice invoice)
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();


        }
    }
}
