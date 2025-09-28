using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.RoleClaimDtos
{
    public class CreateRoleClaimDto
    {
        public string RoleId { get; set; } = string.Empty;
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
    }
}
