using System.ComponentModel.DataAnnotations;

namespace Clinic.API.API.Dtos.RoleDtos
{
    public class CreateRoleDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
