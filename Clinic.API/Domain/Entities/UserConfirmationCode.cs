using Clinic.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Clinic.API.Domain.Entities
{
    public class UserConfirmationCode
    {
        public int Id { get; set; }
        [MaxLength(450)]

        public string UserId { get; set; } = string.Empty;
        [MaxLength(6)] 
        public string Code { get; set; } = string.Empty;
        public ConfirmationType Type { get; set; }
        public DateTime ExpireAt { get; set; }
        public bool IsUsed { get; set; }
    }
}