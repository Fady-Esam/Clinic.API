using Clinic.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos.AppointmentDtos
{
    public class UpdateAppointmentDto
    {   
        public DateTime? AppointmentDate { get; set; }

        [EnumDataType(typeof(AppointmentStatus), ErrorMessage = "Invalid appointment status")]
        public AppointmentStatus? Status { get; set; }

        [MaxLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
        public string? Notes { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? DoctorId { get; set; }
    }
}
