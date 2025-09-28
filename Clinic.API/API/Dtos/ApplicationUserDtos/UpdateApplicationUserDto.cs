using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.ApplicationUserDtos
{
    public class UpdateApplicationUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

}
