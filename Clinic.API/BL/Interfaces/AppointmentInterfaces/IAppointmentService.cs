using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.AppointmentDtos;
using Clinic.API.DL.Models;

namespace Clinic.API.BL.Interfaces.AppointmentInterfaces
{
    public interface IAppointmentService
    {
        Task<ApiResponse<AppointmentDto>> CreateAsync(CreateAppointmentDto dto);
        Task<ApiResponse<AppointmentDto>> UpdateAsync(Guid id, UpdateAppointmentDto dto);
        Task<ApiResponse<bool>> DeleteAsync(Guid id);
        Task<ApiResponse<AppointmentDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IReadOnlyList<AppointmentDto>>> GetAllAsync();
        Task<ApiResponse<PagedResult<AppointmentDto>>> GetPagedAsync(PagingDto dto);
    }

}
