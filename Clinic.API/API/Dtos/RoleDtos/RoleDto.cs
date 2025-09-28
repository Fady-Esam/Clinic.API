namespace Clinic.API.API.Dtos.RoleDtos
{
    public class RoleDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
        public string? ConcurrencyStamp { get; set; }
    }
}
