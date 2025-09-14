using Clinic.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.API.Domain.Entities
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); 

        [Required(ErrorMessage = "Appointment date is required")]
        public DateTime AppointmentDate { get; set; }

        [EnumDataType(typeof(AppointmentStatus), ErrorMessage = "Invalid appointment status")]
        public AppointmentStatus Status { get; set; }

        [MaxLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } 

        [Required(ErrorMessage = "PatientId is required")]
        [ForeignKey(nameof(PatientId))]
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        [Required(ErrorMessage = "DoctorId is required")]
        [ForeignKey(nameof(DoctorId))]
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;
    }

}
