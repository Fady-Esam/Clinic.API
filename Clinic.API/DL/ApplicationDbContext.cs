using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Clinic.API.Domain.Entities;

namespace Clinic.API.DL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict); // prevent multiple cascade paths

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.ApplicationUser)
                .WithOne()
                .HasForeignKey<Doctor>(d => d.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient → ApplicationUser
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.ApplicationUser)
                .WithOne()
                .HasForeignKey<Patient>(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
            // Patient
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Patient)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<Patient>(p => p.ApplicationUserId);

            // Doctor
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Doctor)
                .WithOne(d => d.ApplicationUser)
                .HasForeignKey<Doctor>(d => d.ApplicationUserId);
        }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<UserConfirmationCode> UserConfirmationCodes { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }

    }
}



