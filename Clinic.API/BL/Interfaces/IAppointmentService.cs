using Clinic.API.API.Dtos.AppointmentDtos;
using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.Common.Responses;

namespace Clinic.API.BL.Interfaces
{
    public interface IAppointmentService
    {
        Task<ApiResponse<AppointmentDto>> CreateAsync(CreateAppointmentDto dto);
        Task<ApiResponse<AppointmentDto>> UpdateAsync(Guid id, UpdateAppointmentDto dto);
        Task<ApiResponse<object>> DeleteAsync(Guid id);
        Task<ApiResponse<AppointmentDto>> GetByIdAsync(Guid id);
        //Task<ApiResponse<AppointmentDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IReadOnlyList<AppointmentDto>>> GetAllAsync();
        Task<ApiResponse<PagedResult<AppointmentDto>>> GetPagedAsync(PagingDto dto);
    }

}
