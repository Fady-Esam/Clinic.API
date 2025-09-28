
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.Common.Extensions;
using Clinic.API.Common.Responses;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.DL.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<Doctor> AddAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }
        public async Task<Doctor> UpdateAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
            return doctor;
        }

        public async Task SoftDeleteAsync(Doctor doctor)
        {
            doctor.IsDeleted = true;
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
            //var doctor = await _context.Doctors.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            //if (doctor == null) return false;

            //doctor.IsDeleted = true;
            //_context.Doctors.Update(doctor);
            //await _context.SaveChangesAsync();
            //return true;
        }

        public async Task<Doctor?> GetByIdAsync(Guid id, bool withInclude = true)
        {
            var query = _context.Doctors.AsNoTracking().AsQueryable();

            if (withInclude)
                query = query.Include(p => p.ApplicationUser);

            return await query.FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        public async Task<IReadOnlyList<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
               .AsNoTracking()
               .Include(p => p.ApplicationUser)
               .Where(p => !p.IsDeleted)
               .ToListAsync();
        }
        public async Task<ICollection<Appointment>> GetAppointmentsAsync(Guid doctorId)
        {
            var doctor = await _context.Doctors
                .AsNoTracking()
                .Include(p => p.Appointments) // eager load appointments
                .FirstOrDefaultAsync(p => p.Id == doctorId && !p.IsDeleted);

            return doctor?.Appointments ?? new List<Appointment>();
        }

        public async Task<PagedResult<Doctor>> GetPagedAsync(PagingDto dto)
        {
            var query = _context.Doctors
                .AsNoTracking()
                .Include(p => p.ApplicationUser) // load navigation property
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Search))
            {
                query = query.Where(p => !string.IsNullOrEmpty(p.ApplicationUser.UserName) &&
                                         p.ApplicationUser.UserName.ToLower().Contains(dto.Search.ToLower()));
            }

            return await query.ToPagedResultAsync(dto.Page, dto.PageSize);

        }

       
    }
}
