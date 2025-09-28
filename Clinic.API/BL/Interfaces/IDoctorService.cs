using Clinic.API.API.Dtos.DoctorDtos;
using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.Common.Responses;

namespace Clinic.API.BL.Interfaces
{
    public interface IDoctorService
    {
        Task<ApiResponse<DoctorDto>> CreateAsync(CreateDoctorDto dto);
        Task<ApiResponse<DoctorDto>> UpdateAsync(Guid id, UpdateDoctorDto dto);
        Task<ApiResponse<object>> DeleteAsync(Guid id);
        Task<ApiResponse<DoctorDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IReadOnlyList<DoctorDto>>> GetAllAsync();
        Task<ApiResponse<PagedResult<DoctorDto>>> GetPagedAsync(PagingDto dto);
    }
}
