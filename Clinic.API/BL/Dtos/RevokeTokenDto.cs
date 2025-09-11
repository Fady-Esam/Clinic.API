using System.ComponentModel.DataAnnotations;

namespace Clinic.API.BL.Dtos
{
    public class RevokeTokenDto
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }
    }
}
