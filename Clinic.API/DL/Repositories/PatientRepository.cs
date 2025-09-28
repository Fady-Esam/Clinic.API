using AutoMapper;
using AutoMapper.QueryableExtensions;
using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Extensions;
using Clinic.API.Common.Responses;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Interfaces;
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
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
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
        public async Task SoftDeleteAsync(Patient patient)
        {

            patient.IsDeleted = true;
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();

            //var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            //if (patient == null) return false;

            //patient.IsDeleted = true;
            //_context.Patients.Update(patient);
            //await _context.SaveChangesAsync();
            //return true;
        }

        public async Task<Patient?> GetByIdAsync(Guid id, bool withInclude = true)
        {

            var query = _context.Patients.AsNoTracking().AsQueryable();

            if (withInclude)
                query = query.Include(p => p.ApplicationUser);

            return await query.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

        }


        public async Task<IReadOnlyList<Patient>> GetAllAsync()
        {
            return await _context.Patients
                .AsNoTracking()
                .Include(p => p.ApplicationUser)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }
        public async Task<ICollection<Appointment>> GetAppointmentsAsync(Guid patientId)
        {
            var patient = await _context.Patients
                .AsNoTracking()
                .Include(p => p.Appointments) // eager load appointments
                .FirstOrDefaultAsync(p => p.Id == patientId && !p.IsDeleted);

            return patient?.Appointments ?? new List<Appointment>();
        }
        public async Task<PagedResult<Patient>> GetPagedAsync(PagingDto dto)
        {
            var query = _context.Patients
                .AsNoTracking()
                .Include(p => p.ApplicationUser) // load navigation property
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Search))
            {
                var search = dto.Search.ToLower();

                query = query.Where(p => !string.IsNullOrEmpty(p.ApplicationUser.UserName) &&
                                         p.ApplicationUser.UserName.ToLower().Contains(search));
            }

            return await query.ToPagedResultAsync(dto.Page, dto.PageSize);
        }

    }

}
