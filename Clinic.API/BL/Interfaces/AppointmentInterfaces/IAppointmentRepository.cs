using Clinic.API.BL.Dtos;
using Clinic.API.Domain.Entities;

namespace Clinic.API.BL.Interfaces.AppointmentInterfaces
{
    public interface IAppointmentRepository
    {
        Task<Appointment> AddAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<Appointment?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Appointment>> GetAllAsync();
        Task<(List<Appointment> Items, int Total)> GetPagedAsync(PagingDto dto);
    }

}
