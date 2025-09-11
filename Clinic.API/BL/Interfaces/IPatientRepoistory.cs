using Clinic.API.Domain.Entities;

namespace Clinic.API.BL.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Patient>> GetAllAsync();
        Task<Patient> AddAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task<bool> SoftDeleteAsync(Guid id);
    }
}
