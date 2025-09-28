using Clinic.API.API.Dtos.PagingDtos;
using Clinic.API.Common.Extensions;
using Clinic.API.Common.Responses;
using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Clinic.API.DL.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
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

        public async Task SoftDeleteAsync(Appointment appointment)
        {

            appointment.IsDeleted = true;
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();

            //var appointment = await _context.Appointments
            //    .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            //if (appointment == null) return false;

            //appointment.IsDeleted = true;
            //_context.Appointments.Update(appointment);
            //await _context.SaveChangesAsync();
            //return true;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id, bool withInclude = true)
        {
            //return await _context.Appointments
            //    .AsNoTracking()
            //    .Include(a => a.Patient)
            //        .ThenInclude(p => p.ApplicationUser) // include user info
            //    .Include(a => a.Doctor)
            //        .ThenInclude(d => d.ApplicationUser)
            //    .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

            var query = _context.Appointments.AsNoTracking().AsQueryable();

            if (withInclude)
                query = query.Include(a => a.Patient)
                    .ThenInclude(p => p.ApplicationUser) 
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.ApplicationUser);

            return await query.FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);

        }


        public async Task<IReadOnlyList<Appointment>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
             .AsNoTracking()
             .Include(a => a.Patient)
             .Where(a => a.PatientId == patientId && !a.IsDeleted)
             .ToListAsync();
        }

        public async Task<IReadOnlyList<Appointment>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId == doctorId && !a.IsDeleted)
                .ToListAsync();
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

        public async Task<PagedResult<Appointment>> GetPagedAsync(PagingDto dto)
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
                var search = dto.Search.ToLower();
                query = query.Where(a =>
                    (!string.IsNullOrEmpty(a.Patient.ApplicationUser.UserName) &&
                     a.Patient.ApplicationUser.UserName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(a.Doctor.ApplicationUser.UserName) &&
                     a.Doctor.ApplicationUser.UserName.ToLower().Contains(search))
                );
            }


            return await query.ToPagedResultAsync(dto.Page, dto.PageSize);
        }

        
    }
}
