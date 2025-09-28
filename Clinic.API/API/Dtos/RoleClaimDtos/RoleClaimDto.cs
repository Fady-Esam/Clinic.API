namespace Clinic.API.API.Dtos.RoleClaimDtos
{
    public class RoleClaimDto
    {
        public int Id { get; set; }
        public string RoleId { get; set; } = string.Empty;
        public string? RoleName { get; set; }  
        public string? ClaimType { get; set; }
        public string? ClaimValue { get; set; } 
    }

}
