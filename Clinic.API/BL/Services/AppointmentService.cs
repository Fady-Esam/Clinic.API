using AutoMapper;
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.AppointmentDtos;
using Clinic.API.BL.Interfaces.AppointmentInterfaces;
using Clinic.API.BL.Interfaces.DoctorInterfaces;
using Clinic.API.BL.Interfaces.PatientInterfaces;
using Clinic.API.DL.Models;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.BL.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly IPatientRepository _patientRepo;
        private readonly IDoctorRepository _doctortRepo;
        private readonly IMapper _mapper;

        public AppointmentService(IAppointmentRepository repo, IPatientRepository patientRepo, IDoctorRepository doctortRepo, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _patientRepo = patientRepo;
            _doctortRepo = doctortRepo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<AppointmentDto>> CreateAsync(CreateAppointmentDto dto)
        {
            const string errMessage = "Failed to create appointment";

            if (dto.AppointmentDate < DateTime.UtcNow.AddHours(-1))
                return ApiResponse<AppointmentDto>.Failure(errMessage, new() { "Appointment date cannot be in the past." });

            var patient = await _patientRepo.GetByIdAsync(dto.PatientId!.Value);
            if (patient == null)
                return ApiResponse<AppointmentDto>.Failure(errMessage, new() { $"Patient with Id {dto.PatientId} not found." }, StatusCodes.Status404NotFound);

            var doctor = await _doctortRepo.GetByIdAsync(dto.DoctorId!.Value);
            if (doctor == null)
                return ApiResponse<AppointmentDto>.Failure(errMessage, new() { $"Doctor with Id {dto.DoctorId} not found." }, StatusCodes.Status404NotFound);

            var patientAppointments = await _patientRepo.GetPatientAppointmentsAsync(dto.PatientId!.Value);
            bool patientBusy = patientAppointments
                .Any(a => a.PatientId == dto.PatientId
                    && Math.Abs((a.AppointmentDate - dto.AppointmentDate!.Value).TotalMinutes) < 15
                    && a.Status != AppointmentStatus.Cancelled);

            if (patientBusy)
                return ApiResponse<AppointmentDto>.Failure(errMessage, new() { "Patient already has an appointment at this date/time." });

            var doctorAppointments = await _doctortRepo.GetDoctorAppointmentsAsync(dto.DoctorId!.Value);
            bool doctorBusy = doctorAppointments
                .Any(a => a.DoctorId == dto.DoctorId
                    && Math.Abs((a.AppointmentDate - dto.AppointmentDate!.Value).TotalMinutes) < 15
                    && a.Status != AppointmentStatus.Cancelled);

            if (doctorBusy)
                return ApiResponse<AppointmentDto>.Failure(errMessage, new() { "Doctor is not available at this date/time." });

            var appointment = _mapper.Map<Appointment>(dto);
            var created = await _repo.AddAsync(appointment);
            created.Patient = patient;
            created.Doctor = doctor;

            return ApiResponse<AppointmentDto>.Success(_mapper.Map<AppointmentDto>(created), "Appointment created successfully", StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<AppointmentDto>> UpdateAsync(Guid id, UpdateAppointmentDto dto)
        {
            const string errMessage = "Failed to update appointment";

            if (id == Guid.Empty)
                return ApiResponse<AppointmentDto>.Failure(errMessage, new() { "Appointment Id must not be empty" });

            var appointment = await _repo.GetByIdAsync(id);
            if (appointment == null)
                return ApiResponse<AppointmentDto>.Failure(errMessage, new() { $"Appointment not found with Id {id}" }, StatusCodes.Status404NotFound);

            Patient? patient = appointment.Patient;
            if (dto.PatientId.HasValue && dto.PatientId.Value != appointment.PatientId)
            {
                patient = await _patientRepo.GetByIdAsync(dto.PatientId.Value);
                if (patient == null)
                    return ApiResponse<AppointmentDto>.Failure(errMessage, new() { $"Patient with Id {dto.PatientId} not found." }, StatusCodes.Status404NotFound);
            }

            Doctor? doctor = appointment.Doctor;
            if (dto.DoctorId.HasValue && dto.DoctorId.Value != appointment.DoctorId)
            {
                doctor = await _doctortRepo.GetByIdAsync(dto.DoctorId.Value);
                if (doctor == null)
                    return ApiResponse<AppointmentDto>.Failure(errMessage, new() { $"Doctor with Id {dto.DoctorId} not found." }, StatusCodes.Status404NotFound);
            }

            if (dto.AppointmentDate.HasValue)
            {
                var patientAppointments = await _patientRepo.GetPatientAppointmentsAsync(dto.PatientId!.Value);
                bool patientBusy = patientAppointments
                    .Any(a => Math.Abs((a.AppointmentDate - dto.AppointmentDate!.Value).TotalMinutes) < 15
                           && a.Status != AppointmentStatus.Cancelled
                           && a.Id != id);
                if (patientBusy)
                    return ApiResponse<AppointmentDto>.Failure(errMessage, new() { "Patient already has an appointment at this date/time." });

                var doctorAppointments = await _doctortRepo.GetDoctorAppointmentsAsync(dto.DoctorId!.Value);
                bool doctorBusy = doctorAppointments
                    .Any(a => Math.Abs((a.AppointmentDate - dto.AppointmentDate!.Value).TotalMinutes) < 15
                           && a.Status != AppointmentStatus.Cancelled
                           && a.Id != id);
                if (doctorBusy)
                    return ApiResponse<AppointmentDto>.Failure(errMessage, new() { "Doctor is not available at this date/time." });
            }

            _mapper.Map(dto, appointment);
            var updated = await _repo.UpdateAsync(appointment);

            return ApiResponse<AppointmentDto>.Success(_mapper.Map<AppointmentDto>(updated), "Appointment updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            const string errMessage = "Failed to delete appointment";

            if (id == Guid.Empty)
                return ApiResponse<bool>.Failure(errMessage, new() { "Appointment Id must not be empty" });

            var success = await _repo.SoftDeleteAsync(id);
            if (!success)
                return ApiResponse<bool>.Failure(errMessage, new() { $"Appointment not found with Id {id}" }, StatusCodes.Status404NotFound);

            return ApiResponse<bool>.Success(true, "Appointment deleted successfully");
        }

        public async Task<ApiResponse<AppointmentDto>> GetByIdAsync(Guid id)
        {
            const string errMessage = "Failed to get appointment";

            if (id == Guid.Empty)
                return ApiResponse<AppointmentDto>.Failure(errMessage, new() { "Appointment Id must not be empty" });

            var appointment = await _repo.GetByIdAsync(id);
            if (appointment == null)
                return ApiResponse<AppointmentDto>.Failure(errMessage, new() { $"Appointment not found with Id {id}" }, StatusCodes.Status404NotFound);

            return ApiResponse<AppointmentDto>.Success(_mapper.Map<AppointmentDto>(appointment), "Appointment fetched successfully");
        }

        public async Task<ApiResponse<IReadOnlyList<AppointmentDto>>> GetAllAsync()
        {
            var appointments = await _repo.GetAllAsync();
            var dtos = _mapper.Map<IReadOnlyList<AppointmentDto>>(appointments);

            return ApiResponse<IReadOnlyList<AppointmentDto>>.Success(dtos, "Appointments fetched successfully");
        }

        public async Task<ApiResponse<PagedResult<AppointmentDto>>> GetPagedAsync(PagingDto dto)
        {
            var (appointments, total) = await _repo.GetPagedAsync(dto);
            var items = _mapper.Map<List<AppointmentDto>>(appointments);

            return ApiResponse<PagedResult<AppointmentDto>>.Success(new PagedResult<AppointmentDto>
            {
                Items = items,
                Total = total,
                Page = dto.Page,
                PageSize = dto.PageSize
            }, "Appointments fetched successfully");
        }
    }

}
