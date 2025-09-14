using Clinic.API.BL.Dtos;
using Clinic.API.BL.Interfaces.AppointmentInterfaces;
using Clinic.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.DL.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDBContext _context;

        public AppointmentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Appointment> AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<bool> SoftDeleteAsync(Guid id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            if (appointment == null) return false;

            appointment.IsDeleted = true;
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                    .ThenInclude(p => p.ApplicationUser) // include user info
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<IReadOnlyList<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                    .ThenInclude(p => p.ApplicationUser)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.ApplicationUser)
                .Where(a => !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<(List<Appointment> Items, int Total)> GetPagedAsync(PagingDto dto)
        {
            var query = _context.Appointments
                .AsNoTracking()
                .Include(a => a.Patient)
                    .ThenInclude(p => p.ApplicationUser)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.ApplicationUser)
                .Where(a => !a.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Search))
            {
                query = query.Where(a =>
                    (a.Patient.ApplicationUser.UserName != null &&
                     a.Patient.ApplicationUser.UserName.Contains(dto.Search)) ||
                    (a.Doctor.ApplicationUser.UserName != null &&
                     a.Doctor.ApplicationUser.UserName.Contains(dto.Search))
                );
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(a => a.AppointmentDate)
                .Skip((dto.Page - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}
