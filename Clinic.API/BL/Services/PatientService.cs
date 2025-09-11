using AutoMapper;
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.DL.Models;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PatientDto> CreateAsync(PatientDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.ApplicationUserId))
                throw new ArgumentException("ApplicationUserId is required.");
            if (await _userManager.FindByIdAsync(dto.ApplicationUserId) == null)
                throw new ArgumentException("User not found");

            if(await _userManager.Users.FirstOrDefaultAsync((u) => u.Id == dto.ApplicationUserId) != null)
                throw new ArgumentException("Patient Already Exists");

            var patient = _mapper.Map<Patient>(dto);
            var created = await _repo.AddAsync(patient);
            return _mapper.Map<PatientDto>(created);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repo.SoftDeleteAsync(id);
        }

        public async Task<IReadOnlyList<PatientDto>> GetAllAsync()
        {
            var patients = await _repo.GetAllAsync();
            return _mapper.Map<IReadOnlyList<PatientDto>>(patients);
        }

        public async Task<PatientDto?> GetByIdAsync(Guid id)
        {
            var patient = await _repo.GetByIdAsync(id);
            return patient == null ? null : _mapper.Map<PatientDto>(patient);
        }

        public async Task<PatientDto?> UpdateAsync(PatientDto dto)
        {
            var patient = await _repo.GetByIdAsync(dto.Id);
            if (patient == null) return null;

            _mapper.Map(dto, patient);
            var updated = await _repo.UpdateAsync(patient);
            return _mapper.Map<PatientDto>(updated);
        }
    }
}
