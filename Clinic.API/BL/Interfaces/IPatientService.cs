using Clinic.API.BL.Dtos;
using Clinic.API.DL.Models;

namespace Clinic.API.BL.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDto?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<PatientDto>> GetAllAsync();
        Task<PatientDto> CreateAsync(PatientDto dto);
        Task<PatientDto?> UpdateAsync(PatientDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}
