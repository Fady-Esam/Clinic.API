namespace Clinic.API.API.Dtos.ApplicationUserDtos
{
    public class ApplicationUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; } 
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
    }
}
