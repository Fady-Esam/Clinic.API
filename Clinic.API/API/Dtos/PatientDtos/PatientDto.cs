using Clinic.API.API.Dtos.ApplicationUserDtos;
using System.Text.Json.Serialization;

namespace Clinic.API.API.Dtos.PatientDtos
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfRegisteration { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        [JsonPropertyName("User")]
        public ApplicationUserDto ApplicationUserDto { get; set; } = null!;
    }
}
