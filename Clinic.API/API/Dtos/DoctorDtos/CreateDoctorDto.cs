using FluentValidation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Clinic.API.API.Dtos.DoctorDtos
{
    public class CreateDoctorDto
    {
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string Specialty { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }

        [JsonPropertyName("UserId")]
        public string ApplicationUserId { get; set; } = string.Empty;
    }

}