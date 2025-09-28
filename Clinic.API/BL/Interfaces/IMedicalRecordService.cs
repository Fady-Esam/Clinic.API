using Clinic.API.API.Dtos.MedicalRecordDtos;
using Clinic.API.Common.Responses;

namespace Clinic.API.BL.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<ApiResponse<MedicalRecordDto>> GetByIdAsync(Guid id);
        Task<ApiResponse<IReadOnlyList<MedicalRecordDto>>> GetForPatientAsync(Guid patientId);
        Task<ApiResponse<IReadOnlyList<MedicalRecordDto>>> GetForDoctorAsync(Guid doctorId);
        Task<ApiResponse<Guid>> CreateAsync(CreateMedicalRecordDto dto);
        Task<ApiResponse<object>> UpdateAsync(Guid id, UpdateMedicalRecordDto dto);
        Task<ApiResponse<object>> DeleteAsync(Guid id);
    }
}
