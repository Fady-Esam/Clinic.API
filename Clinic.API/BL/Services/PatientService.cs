using AutoMapper;
using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.API.Dtos.PatientDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Clinic.API.DL.Repositories;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;
using Clinic.API.Domain.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Numerics;
using static StackExchange.Redis.Role;
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

        public async Task<ApiResponse<PatientDto>> CreateAsync(CreatePatientDto dto)
        {
            const string errMessage = "Failed to create patient";
            var user = await _userManager.Users
                                         .AsNoTracking()
                                         .Include(u => u.Patient)
                                         .Include(u => u.Doctor)
                                         .FirstOrDefaultAsync(u => u.Id == dto.ApplicationUserId);

            if (user == null)
                return ApiResponse<PatientDto>.Failure(errMessage, new() { $"User not found with Id {dto.ApplicationUserId}" }, StatusCodes.Status404NotFound);

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Any(r => string.Equals(r, UserRole.Patient.ToString(), StringComparison.OrdinalIgnoreCase)))
                return ApiResponse<PatientDto>.Failure(errMessage, new() { $"Roles for user Id {dto.ApplicationUserId} do not contain Patient" });

            if (user.Patient != null)
                return ApiResponse<PatientDto>.Failure(errMessage, new() { $"Patient already exists for user Id {dto.ApplicationUserId}" });

            if (user.Doctor != null)
                return ApiResponse<PatientDto>.Failure(errMessage, new() { $"This Id {dto.ApplicationUserId} is already registered as Doctor" });

            var patient = _mapper.Map<Patient>(dto);
            var createdPatient = await _repo.AddAsync(patient);
            createdPatient.ApplicationUser = user;

            return ApiResponse<PatientDto>.Success(_mapper.Map<PatientDto>(createdPatient), "Patient created successfully", StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<PatientDto>> UpdateAsync(Guid id, UpdatePatientDto dto)
        {
            const string errMessage = "Failed to update patient";

            if (id == Guid.Empty)
                return ApiResponse<PatientDto>.Failure(errMessage, new() { "Patient Id must not be empty" });

            var patient = await _repo.GetByIdAsync(id);
            if (patient == null)
                return ApiResponse<PatientDto>.Failure(errMessage, new() { $"Patient not found with Id {id}" }, StatusCodes.Status404NotFound);

            _mapper.Map(dto, patient);
            var updated = await _repo.UpdateAsync(patient);

            if (dto.UpdateApplicationUserDto != null)
            {
                var user = patient.ApplicationUser;

                if (!string.IsNullOrEmpty(dto.UpdateApplicationUserDto.UserName))
                {
                    var usernameExists = await _userManager.Users
                        .AsNoTracking()
                        .AnyAsync(u => u.NormalizedUserName == dto.UpdateApplicationUserDto.UserName.ToUpper() && u.Id != user.Id);
                    if (usernameExists) return ApiResponse<PatientDto>.Failure(errMessage, new() { "Username is already taken." });
                }

                if (!string.IsNullOrEmpty(dto.UpdateApplicationUserDto.Email))
                {
                    var emailExists = await _userManager.Users
                        .AsNoTracking()
                        .AnyAsync(u => u.NormalizedEmail == dto.UpdateApplicationUserDto.Email.ToUpper() && u.Id != user.Id);
                    if (emailExists) return ApiResponse<PatientDto>.Failure(errMessage, new() { "Email is already taken." });
                }

                if (!string.IsNullOrEmpty(dto.UpdateApplicationUserDto.PhoneNumber))
                {
                    var phoneNumberExists = await _userManager.Users
                        .AsNoTracking()
                        .AnyAsync(u => u.PhoneNumber == dto.UpdateApplicationUserDto.PhoneNumber && u.Id != user.Id);
                    if (phoneNumberExists) return ApiResponse<PatientDto>.Failure(errMessage, new() { "Phone Number is already taken." });
                }

                _mapper.Map(dto.UpdateApplicationUserDto, user);
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return ApiResponse<PatientDto>.Failure(errMessage, result.Errors.Select(e => e.Description).ToList());
            }

            return ApiResponse<PatientDto>.Success(_mapper.Map<PatientDto>(updated), "Patient updated successfully");
        }

        public async Task<ApiResponse<object>> DeleteAsync(Guid id)
        {
            const string errMessage = "Failed to delete patient";

            if (id == Guid.Empty)
                return ApiResponse<object>.Failure(errMessage, new() { "Patient Id must not be empty" });

            var patient = await _repo.GetByIdAsync(id, false);
            if (patient == null) return ApiResponse<object>.Failure(errMessage, new() { $"Patient not found with Id {id}" }, StatusCodes.Status404NotFound);

            return ApiResponse<object>.SuccessNoData("Patient deleted successfully");
        }

        public async Task<ApiResponse<PatientDto>> GetByIdAsync(Guid id)
        {
            const string errMessage = "Failed to get patient";

            if (id == Guid.Empty) return ApiResponse<PatientDto>.Failure(errMessage, new() { "Patient Id must not be empty" });

            var patient = await _repo.GetByIdAsync(id);
            if (patient == null) return ApiResponse<PatientDto>.Failure(errMessage, new() { $"Patient not found with Id {id}" }, StatusCodes.Status404NotFound);

            return ApiResponse<PatientDto>.Success(_mapper.Map<PatientDto>(patient), "Patient fetched successfully");
        }

        public async Task<ApiResponse<IReadOnlyList<PatientDto>>> GetAllAsync()
        {
            var patients = await _repo.GetAllAsync();
            var dtos = _mapper.Map<IReadOnlyList<PatientDto>>(patients);

            return ApiResponse<IReadOnlyList<PatientDto>>.Success(dtos, "Patients fetched successfully");
        }

        public async Task<ApiResponse<PagedResult<PatientDto>>> GetPagedAsync(PagingDto dto)
        {

            var result = await _repo.GetPagedAsync(dto);

            var paged = new PagedResult<PatientDto>
            {
                Items = _mapper.Map<List<PatientDto>>(result.Items),
                Total = result.Total,
                Page = result.Page,
                PageSize = result.PageSize
            };

            return ApiResponse<PagedResult<PatientDto>>.Success(paged, "Patients fetched successfully");
        }
    }


}
