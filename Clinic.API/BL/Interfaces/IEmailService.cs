

namespace Clinic.API.BL.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmationCodeAsync(string email, string code, string subject = "Clinic API - Confirmation Code");
        Task SendPasswordResetCodeAsync(string email, string code);
    }
}
