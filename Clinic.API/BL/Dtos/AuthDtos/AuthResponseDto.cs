namespace Clinic.API.BL.Dtos.AuthDtos
{
    public class AuthResponseDto 
    {
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }     
        public string? Email { get; set; } 
        public string? PhoneNumber { get; set; }           
        public List<string> Roles { get; set; } = new();
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiration { get; set; } 
        public DateTime RefreshTokenExpiration { get; set; } 
    }
}
