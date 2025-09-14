using Clinic.API.BL.Dtos.DoctorDtos;
using Clinic.API.BL.Dtos.PatientDtos;
using Clinic.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos.AppointmentDtos
{
    public class CreateAppointmentDto
    {

        [Required(ErrorMessage = "Appointment date is required")]
        public DateTime? AppointmentDate { get; set; } 

        [MaxLength(500, ErrorMessage = "Notes must not exceed 500 characters")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "PatientId is required")]
        public Guid? PatientId { get; set; }

        [Required(ErrorMessage = "DoctorId is required")]
        public Guid? DoctorId { get; set; } 

    }

}
