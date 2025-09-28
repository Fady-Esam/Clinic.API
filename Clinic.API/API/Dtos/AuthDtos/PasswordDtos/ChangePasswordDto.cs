using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.AuthDtos.PasswordDtos
{
    public class ChangePasswordDto
    {
        public string UserId { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
