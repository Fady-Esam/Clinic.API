using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.UserClaimDtos
{
    public class CreateUserClaimDto
    {
        public string UserId { get; set; } = string.Empty;
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
    }
}
