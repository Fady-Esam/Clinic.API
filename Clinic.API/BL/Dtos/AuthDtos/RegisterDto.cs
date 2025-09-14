using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos.AuthDtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        [MaxLength(30, ErrorMessage = "UserName must not exceed 30 charcters")]
        public string UserName { get; set; } = string.Empty;
        [EmailAddress(ErrorMessage = "Email Address must be valid")]
        public string? Email { get; set; }
        [Phone(ErrorMessage = "Phone Number must be valid")]
        public string? PhoneNumber { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "Roles list is required")]
        [MinLength(1, ErrorMessage = "At least one role must be specified")]
        public List<string>? Roles { get; set; } 
    }
}
