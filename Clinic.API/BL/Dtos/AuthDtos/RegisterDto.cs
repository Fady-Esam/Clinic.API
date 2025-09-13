using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos.AuthDtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        [MaxLength(30, ErrorMessage = "UserName must not exceed 30 charcters")]
        public string Username { get; set; }
        [EmailAddress(ErrorMessage = "Email Address must be valid")]
        public string? Email { get; set; }
        [Phone(ErrorMessage = "Phone Number must be valid")]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }
        [MaxLength(30, ErrorMessage = "RoleName must not exceed 30 charcters")]
        public string RoleName { get; set; }
    }
}
