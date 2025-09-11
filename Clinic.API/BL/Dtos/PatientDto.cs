using Clinic.API.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.API.BL.Dtos
{
    public class PatientDto
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfRegisteration { get; set; }
        [MaxLength(10, ErrorMessage = "Gender Length must not exceed 10 characters")]
        public string? Gender { get; set; }
        [MaxLength(200, ErrorMessage = "Address Length must not exceed 200 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Application User Id is required")]
        public string ApplicationUserId { get; set; } = string.Empty;

    }
}
