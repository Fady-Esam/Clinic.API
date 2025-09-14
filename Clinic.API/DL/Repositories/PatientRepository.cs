using AutoMapper;
using AutoMapper.QueryableExtensions;
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.DoctorDtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.BL.Interfaces.PatientInterfaces;
using Clinic.API.Domain.Entities;
using Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Linq;
namespace Clinic.API.DL.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDBContext _context;

        public PatientRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        
        public async Task<Patient> AddAsync(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
            return patient;
        }
        public async Task<Patient> UpdateAsync(Patient patient)
        {
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return patient;
        }
        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (patient == null) return false;

            patient.IsDeleted = true;
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Patient?> GetByIdAsync(Guid id)
        {
            return await _context.Patients
                .AsNoTracking()
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }


        public async Task<IReadOnlyList<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .AsNoTracking()
                .Include(p => p.ApplicationUser)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }
        public async Task<(List<Patient> Items, int Total)> GetPagedAsync(PagingDto dto)
        {
            var query = _context.Patients
                .AsNoTracking()
                .Include(p => p.ApplicationUser) // load navigation property
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Search))
            {
                query = query.Where(p => p.ApplicationUser.UserName != null &&
                                         p.ApplicationUser.UserName.Contains(dto.Search));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.ApplicationUser.UserName ?? "") 
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<ICollection<Appointment>> GetPatientAppointmentsAsync(Guid patientId)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .Include(p => p.Appointments) // eager load appointments
                .FirstOrDefaultAsync(p => p.Id == patientId && !p.IsDeleted);

            return patient?.Appointments ?? new List<Appointment>();
        }
    }

}
