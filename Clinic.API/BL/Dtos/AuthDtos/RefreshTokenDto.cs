
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Clinic.API.BL.Dtos.AuthDtos
{
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(7);
        public string? CreatedByIp { get; set; }
    }
}
