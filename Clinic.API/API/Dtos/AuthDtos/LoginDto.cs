using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.AuthDtos
{
    public class LoginDto
    {
        public string? UserName { get; set; } 
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
