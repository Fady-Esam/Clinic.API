using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.UserClaimDtos
{
    public class UpdateUserClaimDto
    {
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; }
    }
}
