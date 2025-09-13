using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Clinic.API.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
