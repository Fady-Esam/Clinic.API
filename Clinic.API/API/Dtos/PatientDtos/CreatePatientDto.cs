using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.PatientDtos
{
    public class CreatePatientDto
    {
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
