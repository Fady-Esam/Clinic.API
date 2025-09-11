using Clinic.API.BL.Interfaces;
using Clinic.API.Domain.Entities;
using Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Clinic.API.DL.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDBContext _context;

        public PatientRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Patient> AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<IReadOnlyList<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .AsNoTracking()
                .Include(p => p.ApplicationUser)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await _context.Patients
                .AsNoTracking()
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<Patient> UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (patient == null) return false;

            patient.IsDeleted = true;
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
