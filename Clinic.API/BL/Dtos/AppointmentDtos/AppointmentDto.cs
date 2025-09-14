using Clinic.API.BL.Dtos.DoctorDtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.Domain.Enums;

namespace Clinic.API.BL.Dtos.AppointmentDtos
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public PatientDto PatientDto { get; set; } = null!;
        public DoctorDto DoctorDto { get; set; } = null!;
    }
}
