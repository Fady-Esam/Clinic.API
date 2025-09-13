using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.API.Domain.Entities
{
    public class Doctor
    {

        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime? DateOfBirth { get; set; }
        public DateTime DateOfRegisteration { get; set; } = DateTime.UtcNow;
        [MaxLength(10, ErrorMessage = "Gender Length must not exceed 10 characters")]
        public string? Gender { get; set; }
        [MaxLength(200, ErrorMessage = "Address Length must not exceed 200 characters")]
        public string? Address { get; set; }

        [MaxLength(100, ErrorMessage = "Specialty Length must not exceed 100 characters")]
        public string? Specialty { get; set; }

        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
        public int? YearsOfExperience { get; set; }
        public bool IsDeleted { get; set; } = false;

        [Required(ErrorMessage = "Application User Id is required")]
        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
