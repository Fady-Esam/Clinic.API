using Clinic.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.API.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public MedicalRecord MedicalRecord { get; set; } = null!;

        public bool IsDeleted { get; set; }

    }

}
