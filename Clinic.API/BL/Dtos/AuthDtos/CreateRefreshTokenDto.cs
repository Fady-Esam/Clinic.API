using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos.AuthDtos
{
    public class CreateRefreshTokenDto
    {
        [Required(ErrorMessage = "RefreshToken is required")]
        public string RefreshToken { get; set; } = string.Empty;
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; } = string.Empty;
    }
}
