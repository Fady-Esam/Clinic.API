using Clinic.API.Domain.Enums;

namespace Clinic.API.BL.Interfaces
{
    public interface IUserConfirmationCodeService
    {
        Task<string> GenerateCodeAsync(string userId, ConfirmationType type);
        Task<bool> ValidateCodeAsync(string userId, string code, ConfirmationType type);
    }
}
