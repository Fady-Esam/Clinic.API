using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.DoctorDtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.Domain.Entities;

namespace Clinic.API.BL.Interfaces.DoctorInterfaces
{
    public interface IDoctorRepository
    {
        Task<Doctor?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Doctor>> GetAllAsync();
        Task<Doctor> AddAsync(Doctor doctor);
        Task<Doctor> UpdateAsync(Doctor doctor);
        Task<bool> SoftDeleteAsync(Guid id);
        Task<(List<Doctor> Items, int Total)> GetPagedAsync(PagingDto dto);

        Task<ICollection<Appointment>> GetDoctorAppointmentsAsync(Guid doctorId);

    }
}
