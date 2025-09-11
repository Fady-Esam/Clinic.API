using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos
{
    public class LoginDto
    {

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(30, ErrorMessage = "Your UserName must not exceed 30 charcters")]
        public string Username { get; set; }

        //[Required(ErrorMessage = "Password is Required")]
        //[EmailAddress(ErrorMessage = "Email Address must be valid")]
        //public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }
    }
}
