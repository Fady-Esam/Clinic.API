using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.UserRoleDtos
{
    public class CreateUserRoleDto
    {
        public List<string> UserIds { get; set; } = new();
        public List<string> RoleIds { get; set; } = new();
    }
}

