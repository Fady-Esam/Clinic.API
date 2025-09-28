
namespace Clinic.API.BL.Interfaces
{
    public interface ISmsService
    {
        Task SendConfirmationCodeAsync(string phoneNumber, string code);
        Task SendPasswordResetCodeAsync(string phoneNumber, string code);
    }
}
