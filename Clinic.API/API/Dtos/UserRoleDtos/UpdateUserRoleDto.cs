using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.UserRoleDtos
{
    public class UpdateUserRoleDto
    {
        public string UserId { get; set; } = string.Empty;
        public List<string> RoleIds { get; set; } = new();
    }
}
