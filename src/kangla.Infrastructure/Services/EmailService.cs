using Resend;
using kangla.Domain.Interfaces;
using DomainEmailMessage = kangla.Domain.Model.EmailMessage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace kangla.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IResend _resend;
        private readonly string _defaultFromEmail;

        public EmailService(ILogger<EmailService> logger, IResend resend, IConfiguration configuration)
        {
            _logger = logger;
            _resend = resend;
            _defaultFromEmail = configuration["EmailSettings:DefaultFromEmail"]
                ?? Environment.GetEnvironmentVariable("EMAIL_SETTINGS_DEFAULT_FROM_EMAIL")
                ?? throw new InvalidOperationException("EmailSettings:DefaultFromEmail is required.");
        }

        public async Task Send(DomainEmailMessage emailMessage)
        {
            _logger.LogInformation("Sending email to {ToAddress} with subject {Subject}", emailMessage.ToAddress, emailMessage.Subject);

            var message = new Resend.EmailMessage
            {
                From = _defaultFromEmail,
                To = { emailMessage.ToAddress },
                Subject = emailMessage.Subject,
                HtmlBody = emailMessage.Body
            };

            await _resend.EmailSendAsync(message);
        }
    }
}
