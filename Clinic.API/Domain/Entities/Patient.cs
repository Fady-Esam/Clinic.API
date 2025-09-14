using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.API.Domain.Entities
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime? DateOfBirth { get; set; }
        public DateTime DateOfRegisteration { get; set; }
        [MaxLength(10, ErrorMessage = "Gender Length must not exceed 10 characters")]
        public string? Gender { get; set; }
        [MaxLength(200, ErrorMessage = "Address Length must not exceed 200 characters")]
        public string? Address { get; set; }
        public bool IsDeleted { get; set; }

        [Required(ErrorMessage = "Application User Id is required")]
        [ForeignKey(nameof(ApplicationUser))]
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
