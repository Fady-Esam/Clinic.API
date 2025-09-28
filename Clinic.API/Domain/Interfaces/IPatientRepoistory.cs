using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.Common.Responses;
using Clinic.API.Domain.Entities;

namespace Clinic.API.Domain.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient> AddAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task SoftDeleteAsync(Patient patient);
        Task<Patient?> GetByIdAsync(Guid id, bool withInclude = true);
        Task<IReadOnlyList<Patient>> GetAllAsync();
        Task<PagedResult<Patient>> GetPagedAsync(PagingDto dto);
        Task<ICollection<Appointment>> GetAppointmentsAsync(Guid patientId);
    }
}
