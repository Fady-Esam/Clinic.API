
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.DoctorDtos;
using Clinic.API.DL.Models;

namespace Clinic.API.BL.Interfaces.DoctorInterfaces
{
    public interface IDoctorService
    {
        Task<ApiResponse<DoctorDto>> CreateAsync(CreateOrUpdateDoctorDto dto);
        Task<ApiResponse<DoctorDto>> UpdateAsync(CreateOrUpdateDoctorDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
        Task<ApiResponse<DoctorDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IReadOnlyList<DoctorDto>>> GetAllAsync();
        Task<ApiResponse<PagedResult<DoctorDto>>> GetPagedAsync(PagingDto dto);
    }
}
