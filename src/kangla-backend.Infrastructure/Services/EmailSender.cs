using Microsoft.AspNetCore.Identity.UI.Services;

//IEmailSender is used for sending Identity emails (user registration, password reset, etc)
public class EmailSender : IEmailSender
{
    private readonly IEmailService _emailService;

    public EmailSender(IEmailService emailService)
    {
        _emailService = emailService;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        EmailMessage emailMessage = new(email,
        subject,
        htmlMessage);

        await _emailService.Send(emailMessage);
    }
}