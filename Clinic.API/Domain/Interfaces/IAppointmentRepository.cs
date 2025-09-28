using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.Common.Responses;
using Clinic.API.Domain.Entities;

namespace Clinic.API.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> AddAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment);
        Task SoftDeleteAsync(Appointment appointment);
        Task<Appointment?> GetByIdAsync(Guid id, bool withInclude = true);
        Task<IReadOnlyList<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId);
        Task<IReadOnlyList<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId);
        Task<IReadOnlyList<Appointment>> GetAllAsync();
        Task<PagedResult<Appointment>> GetPagedAsync(PagingDto dto);
    }

}
