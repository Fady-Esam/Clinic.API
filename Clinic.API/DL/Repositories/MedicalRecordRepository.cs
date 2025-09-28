using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.DL.Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicalRecordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MedicalRecord> AddAsync(MedicalRecord medicalRecord)
        {
            await _context.MedicalRecords.AddAsync(medicalRecord);
            await _context.SaveChangesAsync();
            return medicalRecord;
        }

        public async Task<MedicalRecord> UpdateAsync(MedicalRecord medicalRecord)
        {
            _context.MedicalRecords.Update(medicalRecord);
            await _context.SaveChangesAsync();
            return medicalRecord;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var recordToDelete = await _context.MedicalRecords.FindAsync(id);
            if (recordToDelete == null)
            {
                return false; // Record not found, deletion failed.
            }

            _context.MedicalRecords.Remove(recordToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MedicalRecord?> GetByIdAsync(Guid id)
        {
            // Eagerly load related patient and doctor details for a comprehensive view.
            return await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .FirstOrDefaultAsync(mr => mr.Id == id);
        }

        public async Task<IReadOnlyList<MedicalRecord>> GetAllAsync()
        {
            // Use AsNoTracking for read-only queries to improve performance.
            return await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<MedicalRecord>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.MedicalRecords
                .Where(mr => mr.PatientId == patientId)
                .Include(mr => mr.Doctor) // Include doctor details for context.
                .OrderByDescending(mr => mr.RecordDate) // Show most recent first.
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<MedicalRecord>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.MedicalRecords
                .Where(mr => mr.DoctorId == doctorId)
                .Include(mr => mr.Patient) // Include patient details for context.
                .OrderByDescending(mr => mr.RecordDate) // Show most recent first.
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
