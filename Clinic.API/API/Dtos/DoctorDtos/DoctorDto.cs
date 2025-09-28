using Clinic.API.API.Dtos.ApplicationUserDtos;
using System.Text.Json.Serialization;

namespace Clinic.API.API.Dtos.DoctorDtos
{
    public class DoctorDto
    {
        public Guid Id { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfRegisteration { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string Specialty { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        [JsonPropertyName("User")]
        public ApplicationUserDto ApplicationUserDto { get; set; } = null!;
    }
}
