using Clinic.API.API.Dtos.ApplicationUserDtos;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Clinic.API.API.Dtos.DoctorDtos
{
    public class UpdateDoctorDto
    {
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Specialty { get; set; }
        public int? YearsOfExperience { get; set; }
        [JsonPropertyName("User")]
        public UpdateApplicationUserDto? UpdateApplicationUserDto { get; set; } 

    }
}
