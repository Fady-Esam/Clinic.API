namespace Clinic.API.API.Dtos.UserClaimDtos
{
    public class UserClaimDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? ClaimType { get; set; } 
        public string? ClaimValue { get; set; }
    }
}
