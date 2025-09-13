using AutoMapper;
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.BL.Interfaces.PatientInterfaces;
using Clinic.API.DL.Models;
using Clinic.API.DL.Repositories;
using Clinic.API.Domain.Entities;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Clinic.API.BL.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientService(IPatientRepository repo, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<ApiResponse<PatientDto>> CreateAsync(CreateOrUpdatePatientDto dto)
        {
            const string errMessage = "Failed to create patient";

            var user = await _userManager.Users
                                         .Include(u => u.Patient)
                                         .Include(d => d.Doctor)
                                         .FirstOrDefaultAsync(u => u.Id == dto.ApplicationUserId);

            if (user == null)
                return new ApiResponse<PatientDto>
                {
                    Message = errMessage,
                    Errors = { $"User not found with Id {dto.ApplicationUserId}" },
                    StatusCode = StatusCodes.Status404NotFound
                };

            if (user.Patient != null)
                return new ApiResponse<PatientDto>
                {
                    Message = errMessage,
                    Errors = { $"Patient already exists for user Id {dto.ApplicationUserId}" },
                    StatusCode = StatusCodes.Status400BadRequest
                };

            if (user.Doctor != null)
                return new ApiResponse<PatientDto>
                {
                    Message = errMessage,
                    Errors = { $"This Id {dto.ApplicationUserId} is already registered as Doctor" },
                    StatusCode = StatusCodes.Status400BadRequest
                };
            var patient = _mapper.Map<Patient>(dto);
            // patient.ApplicationUserId = dto.ApplicationUserId;
            var created = await _repo.AddAsync(patient);

            return new ApiResponse<PatientDto>
            {
                Data = _mapper.Map<PatientDto>(created),
                Message = "Patient created successfully",
                StatusCode = StatusCodes.Status201Created
            };
        }

        public async Task<ApiResponse<PatientDto>> UpdateAsync(CreateOrUpdatePatientDto dto)
        {
            const string errMessage = "Failed to update patient";

            if (dto.Id == Guid.Empty)
                return new ApiResponse<PatientDto>
                {
                    Message = errMessage,
                    Errors = { "Patient Id must not be empty" },
                    StatusCode = StatusCodes.Status400BadRequest
                };

            var patient = await _repo.GetByIdAsync(dto.Id);
            if (patient == null)
                return new ApiResponse<PatientDto>
                {
                    Message = errMessage,
                    Errors = { $"Patient not found with Id {dto.Id}" },
                    StatusCode = StatusCodes.Status404NotFound
                };

            _mapper.Map(dto, patient);
            var updated = await _repo.UpdateAsync(patient);

            return new ApiResponse<PatientDto>
            {
                Data = _mapper.Map<PatientDto>(updated),
                Message = "Patient updated successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            const string errMessage = "Failed to delete patient";

            if (id == Guid.Empty)
                return new ApiResponse<bool>
                {
                    Message = errMessage,
                    Errors = { "Patient Id must not be empty" },
                    StatusCode = StatusCodes.Status400BadRequest
                };

            var success = await _repo.SoftDeleteAsync(id);
            if (!success)
                return new ApiResponse<bool>
                {
                    Message = errMessage,
                    Errors = { $"Patient not found with Id {id}" },
                    StatusCode = StatusCodes.Status404NotFound
                };

            return new ApiResponse<bool>
            {
                Data = success,
                Message = "Patient deleted successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<ApiResponse<PatientDto>> GetByIdAsync(Guid id)
        {
            const string errMessage = "Failed to get patient";

            if (id == Guid.Empty)
                return new ApiResponse<PatientDto>
                {
                    Message = errMessage,
                    Errors = { "Patient Id must not be empty" },
                    StatusCode = StatusCodes.Status400BadRequest
                };

            var patient = await _repo.GetByIdAsync(id);
            if (patient == null)
                return new ApiResponse<PatientDto>
                {
                    Message = errMessage,
                    Errors = { $"Patient not found with Id {id}" },
                    StatusCode = StatusCodes.Status404NotFound
                };

            return new ApiResponse<PatientDto>
            {
                Data = _mapper.Map<PatientDto>(patient),
                Message = "Patient fetched successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }

        public async Task<ApiResponse<IReadOnlyList<PatientDto>>> GetAllAsync()
        {
            var patients = await _repo.GetAllAsync();
            var dtos = _mapper.Map<IReadOnlyList<PatientDto>>(patients);

            return new ApiResponse<IReadOnlyList<PatientDto>>
            {
                Data = dtos,
                Message = "Patients fetched successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }
        public async Task<ApiResponse<PagedResult<PatientDto>>> GetPagedAsync(PagingDto dto)
        {
            var (patients, total) = await _repo.GetPagedAsync(dto);

            var items = _mapper.Map<List<PatientDto>>(patients);

            return new ApiResponse<PagedResult<PatientDto>>
            {
                Data = new PagedResult<PatientDto>
                {
                    Items = items,
                    Total = total,
                    Page = dto.Page,
                    PageSize = dto.PageSize
                },
                Message = "Patients fetched successfully",
                StatusCode = StatusCodes.Status200OK
            };
        }


    }

}
