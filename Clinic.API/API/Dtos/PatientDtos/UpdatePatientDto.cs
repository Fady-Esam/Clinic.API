using Clinic.API.API.Dtos.ApplicationUserDtos;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Clinic.API.API.Dtos.PatientDtos
{
    public class UpdatePatientDto 
    {
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        [JsonPropertyName("User")]
        public UpdateApplicationUserDto? UpdateApplicationUserDto { get; set; }

    }
}
