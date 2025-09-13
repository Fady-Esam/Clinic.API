namespace Clinic.API.BL.Dtos.AuthDtos
{
    public class AuthResponseDto 
    {
        public string UserId { get; set; }         
        public string UserName { get; set; }       
        public string Email { get; set; }           
        public List<string> Roles { get; set; } = new();
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? AccessTokenExpiration { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new();
        public int? StatusCode { get; set; }

    }
}
