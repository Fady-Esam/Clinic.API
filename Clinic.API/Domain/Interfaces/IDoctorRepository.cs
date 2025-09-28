using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.Common.Responses;
using Clinic.API.Domain.Entities;

namespace Clinic.API.Domain.Interfaces
{
    public interface IDoctorRepository
    {
        Task<Doctor> AddAsync(Doctor doctor);
        Task<Doctor> UpdateAsync(Doctor doctor);
        Task SoftDeleteAsync(Doctor doctor);
        Task<Doctor?> GetByIdAsync(Guid id, bool withInclude = true);
        Task<IReadOnlyList<Doctor>> GetAllAsync();
        Task<PagedResult<Doctor>> GetPagedAsync(PagingDto dto);
        Task<ICollection<Appointment>> GetAppointmentsAsync(Guid doctorId);
    }
}
