using Clinic.API.Domain.Entities;
namespace Clinic.API.Domain.Interfaces
{
    public interface IMedicalRecordRepository
    {
        Task<MedicalRecord> AddAsync(MedicalRecord medicalRecord);
        Task<MedicalRecord> UpdateAsync(MedicalRecord medicalRecord);
        Task<bool> DeleteAsync(Guid id);
        Task<MedicalRecord?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<MedicalRecord>> GetAllAsync();
        Task<IReadOnlyList<MedicalRecord>> GetByPatientIdAsync(Guid patientId);
        Task<IReadOnlyList<MedicalRecord>> GetByDoctorIdAsync(Guid doctorId);
    }
}


