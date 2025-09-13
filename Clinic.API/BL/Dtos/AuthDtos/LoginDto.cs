using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos.AuthDtos
{
    public class LoginDto
    {

        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        [MaxLength(30, ErrorMessage = "UserName must not exceed 30 charcters")]
        public string Username { get; set; }

        //[Required(ErrorMessage = "Password is Required")]
        //[EmailAddress(ErrorMessage = "Email Address must be valid")]
        //public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }
    }
}
