using Clinic.API.API.Dtos.DoctorDtos;
using Clinic.API.API.Dtos.PatientDtos;
using Clinic.API.Domain.Enums;
using System.Text.Json.Serialization;

namespace Clinic.API.API.Dtos.AppointmentDtos
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [JsonPropertyName("Patient")]
        public PatientDto PatientDto { get; set; } = null!;
        [JsonPropertyName("Doctor")]
        public DoctorDto DoctorDto { get; set; } = null!;
    }
}
