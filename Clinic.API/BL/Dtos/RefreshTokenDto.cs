using Clinic.API.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Clinic.API.BL.Dtos
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "RefreshToken is required")]
        public string RefreshToken { get; set; }
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }

        public DateTime? Expires { get; set; }
        public DateTime? Created { get; set; }
        public string? CreatedByIp { get; set; }

        [JsonIgnore]
        public bool IsExpired => DateTime.UtcNow >= Expires;
    }

}
