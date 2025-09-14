


using Clinic.API.BL.Dtos.ApplicationUserDtos;

namespace Clinic.API.BL.Dtos.PatientDtos
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime DateOfRegisteration { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public ApplicationUserDto ApplicationUserDto { get; set; } = null!;
    }
}
