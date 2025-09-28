using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic.API.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfRegisteration { get; set; }
        [MaxLength(10)]
        public string Gender { get; set; } = string.Empty;
        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;
        
        [Column("UserId")]
        public string ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public bool IsDeleted { get; set; }

    }
}
