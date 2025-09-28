using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.AuthDtos.PasswordDtos
{
    public class PasswordResetConfirmDto
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Code { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
