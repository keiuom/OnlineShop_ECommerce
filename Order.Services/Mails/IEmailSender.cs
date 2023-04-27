namespace Order.Services.Mails
{
    public interface IEmailSender
    {
        Task SendAsync(MailRequest mailRequest);
    }
}
