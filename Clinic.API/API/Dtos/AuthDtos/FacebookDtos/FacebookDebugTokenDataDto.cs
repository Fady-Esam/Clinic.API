namespace Clinic.API.API.Dtos.AuthDtos.FacebookDtos
{
    public class FacebookDebugTokenDataDto
    {
        public bool IsValid { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string AppId { get; set; } = string.Empty;
    }
}
