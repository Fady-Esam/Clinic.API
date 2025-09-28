using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.RoleClaimDtos
{
    public class UpdateRoleClaimDto
    {
        public string? ClaimType { get; set; } 
        public string? ClaimValue { get; set; } 
    }
}
