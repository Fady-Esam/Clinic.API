
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Clinic.API.BL.Dtos;
using Clinic.API.BL.Dtos.DoctorDtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.BL.Interfaces.DoctorInterfaces;
using Clinic.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.DL.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDBContext _context;

        public DoctorRepository(ApplicationDBContext context)
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

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
            if (doctor == null) return false;

            doctor.IsDeleted = true;
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
            return true;


        }

        public async Task<Doctor?> GetByIdAsync(Guid id)
        {
            return await _context.Doctors
                .AsNoTracking()
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }


        public async Task<IReadOnlyList<Doctor>> GetAllAsync()
        {
            return await _context.Doctors
               .AsNoTracking()
               .Include(p => p.ApplicationUser)
               .Where(p => !p.IsDeleted)
               .ToListAsync();
        }

      
        public async Task<(List<Doctor> Items, int Total)> GetPagedAsync(PagingDto dto)
        {
            var query = _context.Doctors
                 .AsNoTracking()
                 .Include(p => p.ApplicationUser) // load navigation property
                 .Where(p => !p.IsDeleted)
                 .AsQueryable();

            // 🔍 Apply search safely
            if (!string.IsNullOrWhiteSpace(dto.Search))
            {
                query = query.Where(p => p.ApplicationUser.UserName != null &&
                                         p.ApplicationUser.UserName.Contains(dto.Search));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.ApplicationUser.UserName ?? "") // null-safe ordering
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<ICollection<Appointment>> GetDoctorAppointmentsAsync(Guid doctorId)
        {
            var doctor = await _context.Doctors
                .AsNoTracking()
                .Include(p => p.Appointments) // eager load appointments
                .FirstOrDefaultAsync(p => p.Id == doctorId && !p.IsDeleted);

            return doctor?.Appointments ?? new List<Appointment>();
        }

    }
}
