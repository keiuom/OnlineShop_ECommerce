namespace Order.Services.Mails
{
    public interface IEmailSender
    {
        Task SendAsync(string recipient, string subject, string body);
    }
}
