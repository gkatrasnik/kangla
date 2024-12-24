using FluentEmail.Core;
using kangla.Domain.Interfaces;
using kangla.Domain.Model;
using Microsoft.Extensions.Logging;

namespace kangla.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IFluentEmailFactory _fluentEmailFactory;

        public EmailService(ILogger<EmailService> logger, IFluentEmailFactory fluentEmailFactory)
        {
            _logger = logger;
            _fluentEmailFactory = fluentEmailFactory;
        }

        public async Task Send(EmailMessage emailMessage)
        {
            _logger.LogInformation("Sending email");
            await _fluentEmailFactory.Create().To(emailMessage.ToAddress)
                .Subject(emailMessage.Subject)
                .Body(emailMessage.Body, true) // true =  HTML format
                .SendAsync();
        }
    }
}