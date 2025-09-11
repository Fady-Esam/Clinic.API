using Clinic.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Clinic.API.Domain.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        [JsonIgnore]
        public Patient? Patient { get; set; }
    }
}
