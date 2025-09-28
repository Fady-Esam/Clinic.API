using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Settings;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
namespace Clinic.API.BL.Services
{
    public class SmsService : ISmsService
    {
        private readonly SmsSetting _smsSetting;
        private readonly ILogger<SmsService> _logger;


        public SmsService(SmsSetting smsSetting, ILogger<SmsService> logger)
        {
            _smsSetting = smsSetting;
            TwilioClient.Init(_smsSetting.AccountSid, _smsSetting.AuthToken);
            _logger = logger;
        }

        public async Task SendConfirmationCodeAsync(string phoneNumber, string code)
        {
            _logger.LogInformation("Sending SMS to {Phone}: Code {Code}", phoneNumber, code);

            var fromNumber = _smsSetting.FromNumber;
            await MessageResource.CreateAsync(
                to: new Twilio.Types.PhoneNumber(phoneNumber),
                from: new Twilio.Types.PhoneNumber(fromNumber),
                body: code
            );
             // Task.CompletedTask;

        }


        public async Task SendPasswordResetCodeAsync(string phoneNumber, string code)
        {
            await SendConfirmationCodeAsync(phoneNumber, code);
        }
       
        
    }

   
}


