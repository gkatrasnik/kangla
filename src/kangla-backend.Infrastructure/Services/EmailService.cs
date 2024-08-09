using FluentEmail.Core;
using Microsoft.Extensions.Logging;

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
