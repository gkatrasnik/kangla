public interface IEmailService
{
    /// <summary>
    /// Send an email.
    /// </summary>
    /// <param name="emailMessage">Message object to be sent</param>
    Task Send(EmailMessage emailMessage);
}