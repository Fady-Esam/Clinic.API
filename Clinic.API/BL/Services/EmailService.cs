using Clinic.API.BL.Interfaces;
using Clinic.API.Common.Settings;
using Humanizer;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Clinic.API.BL.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSetting _emailSetting;
        private readonly ILogger<EmailService> _logger;

        public EmailService(EmailSetting emailSetting, ILogger<EmailService> logger)
        {
            _emailSetting = emailSetting;
            _logger = logger;
        }

        public async Task SendConfirmationCodeAsync(string email, string code, string subject = "Clinic API - Confirmation Code")
        {
            var body = $@"
            <p>Hello,</p>
            <p>Your confirmation code is: <b>{code}</b></p>
            <p>This code will expire in 2 minutes.</p>";

            // Example using SMTP client (configure in appsettings.json)
            var smtpHost = _emailSetting.SmtpHost;
            var smtpPort = _emailSetting.SmtpPort;
            var smtpUser = _emailSetting.SmtpUser;
            var smtpPass = _emailSetting.SmtpPass;
            var fromEmail = _emailSetting.FromEmail;

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(fromEmail!, email, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Sent confirmation code {Code} to {Email}", code, email);
        }

        public async Task SendPasswordResetCodeAsync(string email, string code)
        {
            await SendConfirmationCodeAsync(email, code, "Clinic API - Password Reset Code");
        }
    }
}
