using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.DL.Models;

namespace Clinic.API.BL.Interfaces.PatientInterfaces
{
    public interface IPatientService
    {
        Task<ApiResponse<PatientDto>> CreateAsync(CreatePatientDto dto);
        Task<ApiResponse<PatientDto>> UpdateAsync(Guid id, UpdatePatientDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
        Task<ApiResponse<PatientDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IReadOnlyList<PatientDto>>> GetAllAsync();
        Task<ApiResponse<PagedResult<PatientDto>>> GetPagedAsync(PagingDto dto);
    }

}
