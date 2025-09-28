using Clinic.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.AppointmentDtos
{
    public class UpdateAppointmentDto
    {   
        public DateTime? AppointmentDate { get; set; }
        public AppointmentStatus? Status { get; set; }
        public string? Notes { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? DoctorId { get; set; }
    }
}
