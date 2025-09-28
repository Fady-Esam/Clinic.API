using AutoMapper;
using Clinic.API.API.Dtos.MedicalRecordDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Interfaces;

namespace Clinic.API.BL.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _recordRepository;
        private readonly IPatientRepository _patientRepository; // For validation
        private readonly IDoctorRepository _doctorRepository;   // For validation
        private readonly IMapper _mapper;

        public MedicalRecordService(
            IMedicalRecordRepository recordRepository,
            IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            IMapper mapper)
        {
            _recordRepository = recordRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<Guid>> CreateAsync(CreateMedicalRecordDto dto)
        {
            // Business Rule: Ensure patient and doctor exist before creating a record.
            if (dto.PatientId != null && await _patientRepository.GetByIdAsync(dto.PatientId.Value) == null)
                return ApiResponse<Guid>.Failure("Validation Failed", new() { "Patient not found." });

            if (dto.DoctorId != null && await _doctorRepository.GetByIdAsync(dto.DoctorId.Value) == null)
                return ApiResponse<Guid>.Failure("Validation Failed", new() { "Doctor not found." });

            // Business Rule: Prevent duplicate records for the same appointment.
            if (dto.AppointmentId != null && await _recordRepository.GetByIdAsync(dto.AppointmentId.Value) != null)
                return ApiResponse<Guid>.Failure("Creation Failed", new() { "A medical record for this appointment already exists." }, StatusCodes.Status409Conflict); // 409 Conflict

            var medicalRecord = _mapper.Map<MedicalRecord>(dto);
            var createdRecord = await _recordRepository.AddAsync(medicalRecord);

            return ApiResponse<Guid>.Success(createdRecord.Id, "Medical record created successfully.", 201);
        }

        public async Task<ApiResponse<object>> UpdateAsync(Guid id, UpdateMedicalRecordDto dto)
        {
            var recordToUpdate = await _recordRepository.GetByIdAsync(id);
            if (recordToUpdate == null)
                return ApiResponse<object>.Failure("Not Found", new() { "Medical record not found." });

            _mapper.Map(dto, recordToUpdate);
            await _recordRepository.UpdateAsync(recordToUpdate);

            return ApiResponse<object>.SuccessNoData("Medical record updated successfully.");
        }

        public async Task<ApiResponse<object>> DeleteAsync(Guid id)
        {
            var success = await _recordRepository.DeleteAsync(id);
            if (!success)
                return ApiResponse<object>.Failure("Not Found", new() { "Medical record not found." });

            return ApiResponse<object>.SuccessNoData("Medical record deleted successfully.");
        }

        public async Task<ApiResponse<MedicalRecordDto>> GetByIdAsync(Guid id)
        {
            var record = await _recordRepository.GetByIdAsync(id);
            if (record == null)
                return ApiResponse<MedicalRecordDto>.Failure("Not Found", new() { "Medical record not found." }, 404);

            // Security logic would be added here to check if the current user
            // is the patient or the doctor associated with this record.
            var recordDto = _mapper.Map<MedicalRecordDto>(record);
            return ApiResponse<MedicalRecordDto>.Success(recordDto, "Record retrieved successfully.");
        }

        public async Task<ApiResponse<IReadOnlyList<MedicalRecordDto>>> GetForPatientAsync(Guid patientId)
        {
            // Security check: ensure the patient exists
            if (await _patientRepository.GetByIdAsync(patientId) != null)
                return ApiResponse<IReadOnlyList<MedicalRecordDto>>.Failure("Not Found", new() { "Patient not found." }, 404);

            var records = await _recordRepository.GetByPatientIdAsync(patientId);
            var recordsDto = _mapper.Map<IReadOnlyList<MedicalRecordDto>>(records);
            return ApiResponse<IReadOnlyList<MedicalRecordDto>>.Success(recordsDto, "Records for patient retrieved successfully.");
        }

        public async Task<ApiResponse<IReadOnlyList<MedicalRecordDto>>> GetForDoctorAsync(Guid doctorId)
        {
            if (await _doctorRepository.GetByIdAsync(doctorId) != null)
                return ApiResponse<IReadOnlyList<MedicalRecordDto>>.Failure("Not Found", new() { "Doctor not found." }, 404);

            var records = await _recordRepository.GetByDoctorIdAsync(doctorId);
            var recordsDto = _mapper.Map<IReadOnlyList<MedicalRecordDto>>(records);
            return ApiResponse<IReadOnlyList<MedicalRecordDto>>.Success(recordsDto, "Records for doctor retrieved successfully.");
        }
    }
}
