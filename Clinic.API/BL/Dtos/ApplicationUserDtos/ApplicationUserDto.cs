namespace Clinic.API.BL.Dtos.ApplicationUserDtos
{
    public class ApplicationUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; } 
        public string? Email { get; set; } 
        public string? PhoneNumber { get; set; } 
    }
}
