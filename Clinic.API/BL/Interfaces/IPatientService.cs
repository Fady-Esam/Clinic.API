using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.API.Dtos.PatientDtos;
using Clinic.API.Common.Responses;

namespace Clinic.API.BL.Interfaces
{
    public interface IPatientService
    {
        Task<ApiResponse<PatientDto>> CreateAsync(CreatePatientDto dto);
        Task<ApiResponse<PatientDto>> UpdateAsync(Guid id, UpdatePatientDto dto);
        Task<ApiResponse<object>> DeleteAsync(Guid id);
        Task<ApiResponse<PatientDto>> GetByIdAsync(Guid id);
        // Task<ApiResponse<PatientDto>> GetByUserIdAsync(string userId);
        Task<ApiResponse<IReadOnlyList<PatientDto>>> GetAllAsync();
        Task<ApiResponse<PagedResult<PatientDto>>> GetPagedAsync(PagingDto dto);
    }

}
