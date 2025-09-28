using Clinic.API.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Clinic.API.API.Dtos.AuthDtos.RefreshTokenDtos
{
    public class RefreshTokenDto
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public DateTime? Revoked { get; set; }
        public string? ReplacedByToken { get; set; }
        public bool IsExpired { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsActive { get; set; }
    }
}


      
