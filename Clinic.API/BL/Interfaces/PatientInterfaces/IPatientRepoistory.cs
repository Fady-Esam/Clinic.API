using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.Domain.Entities;

namespace Clinic.API.BL.Interfaces.PatientInterfaces
{
    public interface IPatientRepository
    {
        Task<Patient> AddAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<Patient?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Patient>> GetAllAsync();
        Task<(List<Patient> Items, int Total)> GetPagedAsync(PagingDto dto);
    }
}
