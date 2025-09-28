using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Clinic.API.API.Dtos.AuthDtos.RefreshTokenDtos
{
    public class CreateNewRefreshTokenDto
    {
        public string Token { get; set; } = string.Empty;
        [JsonIgnore]
        public string UserId { get; set; } = string.Empty;
    }
}
