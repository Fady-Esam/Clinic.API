using Clinic.API.Domain.Entities;
using Clinic.API.Domain.Enums;

namespace Clinic.API.Domain.Interfaces
{
    public interface IUserConfirmationCodeRepository
    {
        Task AddAsync(UserConfirmationCode confirmation);
        Task<UserConfirmationCode?> GetLatestAsync(string userId, ConfirmationType type, string code);
    }
}
