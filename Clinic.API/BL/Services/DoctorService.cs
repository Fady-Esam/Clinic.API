using AutoMapper;
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.DoctorDtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.BL.Interfaces.DoctorInterfaces;
using Clinic.API.DL.Models;
using Clinic.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.BL.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public DoctorService(IDoctorRepository repo, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<ApiResponse<DoctorDto>> CreateAsync(CreateOrUpdateDoctorDto dto)
        {
            const string errMessage = "Failed to create doctor";

            var user = await _userManager.Users
                                         .Include(u => u.Doctor)
                                         .Include(p => p.Patient)
                                         .FirstOrDefaultAsync(u => u.Id == dto.ApplicationUserId);

            if (user == null)
                return new ApiResponse<DoctorDto>
                {
                    Message = errMessage,
                    Errors = { $"User not found with Id {dto.ApplicationUserId}" },
                    StatusCode = StatusCodes.Status404NotFound
                };

            if (user.Doctor != null)
                return new ApiResponse<DoctorDto>
                {
                    Message = errMessage,
                    Errors = { $"Doctor already exists for user Id {dto.ApplicationUserId}" },
                    StatusCode = StatusCodes.Status400BadRequest
                };

            if (user.Patient != null)
                return new ApiResponse<DoctorDto>
                {
                    Message = errMessage,
                    Errors = { $"This Id {dto.ApplicationUserId} is already registered as Patient" },
                    StatusCode = StatusCodes.Status400BadRequest
                };

            var doctor = _mapper.Map<Doctor>(dto);
            // doctor.ApplicationUserId = dto.ApplicationUserId;

            var created = await _repo.AddAsync(doctor);

            return new ApiResponse<DoctorDto>
            {
                Data = _mapper.Map<DoctorDto>(created),
                Message = "Doctor created successfully",
                StatusCode = StatusCodes.Status201Created
            };
        }

        public async Task<ApiResponse<DoctorDto>> UpdateAsync(CreateOrUpdateDoctorDto dto)
        {
            const string errMessage = "Failed to update doctor";

            if (dto.Id == Guid.Empty)
                return new ApiResponse<DoctorDto>
                {
                    Message = errMessage,
                    Errors = { "Doctor Id must not be empty" },
                    StatusCode = StatusCodes.Status400BadRequest
                };

            var doctor = await _repo.GetByIdAsync(dto.Id);
            if (doctor == null)
                return new ApiResponse<DoctorDto>
                {
                    Message = errMessage,
                    Errors = { $"Doctor not found with Id {dto.Id}" },
                    StatusCode = StatusCodes.Status404NotFound
                };

            _mapper.Map(dto, doctor);
            var updated = await _repo.UpdateAsync(doctor);

            return new ApiResponse<DoctorDto>
            {
                Data = _mapper.Map<DoctorDto>(updated),
                Message = "Doctor updated successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            const string errMessage = "Failed to delete doctor";

            if (id == Guid.Empty)
                return new ApiResponse<bool>
                {
                    Message = errMessage,
                    Errors = { "Doctor Id must not be empty" },
                    StatusCode = StatusCodes.Status400BadRequest
                };

            var success = await _repo.SoftDeleteAsync(id);
            if (!success)
                return new ApiResponse<bool>
                {
                    Message = errMessage,
                    Errors = { $"Doctor not found with Id {id}" },
                    StatusCode = StatusCodes.Status404NotFound
                };

            return new ApiResponse<bool>
            {
                Data = success,
                Message = "Doctor deleted successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<ApiResponse<DoctorDto>> GetByIdAsync(Guid id)
        {
            const string errMessage = "Failed to get doctor";

            if (id == Guid.Empty)
                return new ApiResponse<DoctorDto>
                {
                    Message = errMessage,
                    Errors = { "Doctor Id must not be empty" },
                    StatusCode = StatusCodes.Status400BadRequest
                };

            var doctor = await _repo.GetByIdAsync(id);
            if (doctor == null)
                return new ApiResponse<DoctorDto>
                {
                    Message = errMessage,
                    Errors = { $"Doctor not found with Id {id}" },
                    StatusCode = StatusCodes.Status404NotFound
                };

            return new ApiResponse<DoctorDto>
            {
                Data = _mapper.Map<DoctorDto>(doctor),
                Message = "Doctor fetched successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<ApiResponse<IReadOnlyList<DoctorDto>>> GetAllAsync()
        {
            var doctors = await _repo.GetAllAsync();
            var dtos = _mapper.Map<IReadOnlyList<DoctorDto>>(doctors);

            return new ApiResponse<IReadOnlyList<DoctorDto>>
            {
                Data = dtos,
                Message = "Doctors fetched successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }
        public async Task<ApiResponse<PagedResult<DoctorDto>>> GetPagedAsync(PagingDto dto)
        {
            var (patients, total) = await _repo.GetPagedAsync(dto);

            var items = _mapper.Map<List<DoctorDto>>(patients);

            return new ApiResponse<PagedResult<DoctorDto>>
            {
                Data = new PagedResult<DoctorDto>
                {
                    Items = items,
                    Total = total,
                    Page = dto.Page,
                    PageSize = dto.PageSize
                },
                Message = "Doctors fetched successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }
    }

}
