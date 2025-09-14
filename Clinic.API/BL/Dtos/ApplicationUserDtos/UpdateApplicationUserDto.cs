using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos.ApplicationUserDtos
{
    public class UpdateApplicationUserDto
    {
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        [MaxLength(30, ErrorMessage = "UserName must not exceed 30 charcters")]
        public string? UserName { get; set; }
        [EmailAddress(ErrorMessage = "Email Address must be valid")]
        public string? Email { get; set; }
        [Phone(ErrorMessage = "Phone Number must be valid")]
        public string? PhoneNumber { get; set; }
    }
}
