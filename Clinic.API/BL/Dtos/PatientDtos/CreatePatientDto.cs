using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos.PatientDtos
{
    public class CreatePatientDto
    {
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(10, ErrorMessage = "Gender length must not exceed 10 characters")]
        public string? Gender { get; set; }

        [MaxLength(200, ErrorMessage = "Address length must not exceed 200 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Application User Id is required")]
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
