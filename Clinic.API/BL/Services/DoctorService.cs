using AutoMapper;
using Clinic.API.API.Dtos.DoctorDtos;
using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Responses;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;
using Clinic.API.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

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

        public async Task<ApiResponse<DoctorDto>> CreateAsync(CreateDoctorDto dto)
        {
            const string errMessage = "Failed to create doctor";

            var user = await _userManager.Users
                                         .AsNoTracking()
                                         .Include(u => u.Doctor)
                                         .Include(u => u.Patient)
                                         .FirstOrDefaultAsync(u => u.Id == dto.ApplicationUserId);

            if (user == null) return ApiResponse<DoctorDto>.Failure(errMessage, new() { $"User not found with Id {dto.ApplicationUserId}" }, StatusCodes.Status404NotFound);

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Any(r => string.Equals(r, UserRole.Doctor.ToString(), StringComparison.OrdinalIgnoreCase)))
                return ApiResponse<DoctorDto>.Failure(errMessage, new() { $"Roles for user Id {dto.ApplicationUserId} do not contain Doctor" });

            if (user.Doctor != null) return ApiResponse<DoctorDto>.Failure(errMessage, new() { $"Doctor already exists for user Id {dto.ApplicationUserId}" });

            if (user.Patient != null) return ApiResponse<DoctorDto>.Failure(errMessage, new() { $"This Id {dto.ApplicationUserId} is already registered as Patient" });

            var doctor = _mapper.Map<Doctor>(dto);
            var createdDoctor = await _repo.AddAsync(doctor);
            createdDoctor.ApplicationUser = user;

            return ApiResponse<DoctorDto>.Success(_mapper.Map<DoctorDto>(createdDoctor), "Doctor created successfully", StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<DoctorDto>> UpdateAsync(Guid id, UpdateDoctorDto dto)
        {
            const string errMessage = "Failed to update doctor";

            if (id == Guid.Empty) return ApiResponse<DoctorDto>.Failure(errMessage, new() { "Doctor Id must not be empty" });

            var doctor = await _repo.GetByIdAsync(id);
            if (doctor == null) return ApiResponse<DoctorDto>.Failure(errMessage, new() { $"Doctor not found with Id {id}" }, StatusCodes.Status404NotFound);

            _mapper.Map(dto, doctor);
            var updated = await _repo.UpdateAsync(doctor);

            if (dto.UpdateApplicationUserDto != null)
            {
                var user = doctor.ApplicationUser;

                if (!string.IsNullOrEmpty(dto.UpdateApplicationUserDto.UserName))
                {
                    var usernameExists = await _userManager.Users
                        .AsNoTracking()
                        .AnyAsync(u => u.NormalizedUserName == dto.UpdateApplicationUserDto.UserName.ToUpper() && u.Id != user.Id);
                    if (usernameExists) return ApiResponse<DoctorDto>.Failure(errMessage, new() { "Username is already taken." });
                }

                if (!string.IsNullOrEmpty(dto.UpdateApplicationUserDto.Email))
                {
                    var emailExists = await _userManager.Users
                        .AsNoTracking()
                        .AnyAsync(u => u.NormalizedEmail == dto.UpdateApplicationUserDto.Email.ToUpper() && u.Id != user.Id);
                    if (emailExists) return ApiResponse<DoctorDto>.Failure(errMessage, new() { "Email is already taken." });
                }

                if (!string.IsNullOrEmpty(dto.UpdateApplicationUserDto.PhoneNumber))
                {
                    var phoneNumberExists = await _userManager.Users
                        .AsNoTracking()
                        .AnyAsync(u => u.PhoneNumber == dto.UpdateApplicationUserDto.PhoneNumber && u.Id != user.Id);
                    if (phoneNumberExists) return ApiResponse<DoctorDto>.Failure(errMessage, new() { "Phone Number is already taken." });
                }

                _mapper.Map(dto.UpdateApplicationUserDto, user);
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return ApiResponse<DoctorDto>.Failure(errMessage, result.Errors.Select(e => e.Description).ToList());
            }

            return ApiResponse<DoctorDto>.Success(_mapper.Map<DoctorDto>(updated), "Doctor updated successfully");
        }

        public async Task<ApiResponse<object>> DeleteAsync(Guid id)
        {
            const string errMessage = "Failed to delete doctor";

            if (id == Guid.Empty) return ApiResponse<object>.Failure(errMessage, new() { "Doctor Id must not be empty" });

            var doctor = await _repo.GetByIdAsync(id, false);
            if (doctor == null) return ApiResponse<object>.Failure(errMessage, new() { $"Doctor not found with Id {id}" }, StatusCodes.Status404NotFound);


            await _repo.SoftDeleteAsync(doctor);

            return ApiResponse<object>.SuccessNoData("Doctor deleted successfully");
        }

        public async Task<ApiResponse<DoctorDto>> GetByIdAsync(Guid id)
        {
            const string errMessage = "Failed to get doctor";

            if (id == Guid.Empty) return ApiResponse<DoctorDto>.Failure(errMessage, new() { "Doctor Id must not be empty" });

            var doctor = await _repo.GetByIdAsync(id);
            if (doctor == null) return ApiResponse<DoctorDto>.Failure(errMessage, new() { $"Doctor not found with Id {id}" }, StatusCodes.Status404NotFound);

            return ApiResponse<DoctorDto>.Success(_mapper.Map<DoctorDto>(doctor), "Doctor fetched successfully");
        }

        public async Task<ApiResponse<IReadOnlyList<DoctorDto>>> GetAllAsync()
        {
            var doctors = await _repo.GetAllAsync();
            var dtos = _mapper.Map<IReadOnlyList<DoctorDto>>(doctors);

            return ApiResponse<IReadOnlyList<DoctorDto>>.Success(dtos, "Doctors fetched successfully");
        }

        public async Task<ApiResponse<PagedResult<DoctorDto>>> GetPagedAsync(PagingDto dto)
        {

            var result = await _repo.GetPagedAsync(dto);

            var paged = new PagedResult<DoctorDto>
            {
                Items = _mapper.Map<List<DoctorDto>>(result.Items),
                Total = result.Total,
                Page = result.Page,
                PageSize = result.PageSize
            };

            return ApiResponse<PagedResult<DoctorDto>>.Success(paged, "Doctors fetched successfully");
            
        }
    }

}
