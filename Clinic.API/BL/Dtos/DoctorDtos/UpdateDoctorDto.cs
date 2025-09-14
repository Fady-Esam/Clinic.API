using Clinic.API.BL.Dtos.ApplicationUserDtos;
using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos.DoctorDtos
{
    public class UpdateDoctorDto
    {
        public DateTime? DateOfBirth { get; set; }

        [MaxLength(10, ErrorMessage = "Gender length must not exceed 10 characters")]
        public string? Gender { get; set; }

        [MaxLength(200, ErrorMessage = "Address length must not exceed 200 characters")]
        public string? Address { get; set; }

        [MaxLength(100, ErrorMessage = "Specialty length must not exceed 100 characters")]
        public string? Specialty { get; set; }

        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50")]
        public int? YearsOfExperience { get; set; }
        public UpdateApplicationUserDto? UpdateApplicationUserDto { get; set; } 

    }
}
