using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Clinic.API.BL.Dtos.PatientDtos
{
    public class CreateOrUpdatePatientDto
    {
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [JsonIgnore]
        public DateTime DateOfRegisteration { get; set; } = DateTime.UtcNow;
        [MaxLength(10, ErrorMessage = "Gender Length must not exceed 10 characters")]
        public string? Gender { get; set; }
        [MaxLength(200, ErrorMessage = "Address Length must not exceed 200 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Application User Id is required")]
        public string ApplicationUserId { get; set; }
    }
}
