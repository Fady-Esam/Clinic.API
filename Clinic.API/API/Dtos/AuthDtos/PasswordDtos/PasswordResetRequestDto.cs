namespace Clinic.API.API.Dtos.AuthDtos.PasswordDtos
{
    public class PasswordResetRequestDto
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
