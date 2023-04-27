namespace Order.Services.Mails
{
    public class MailRequest
    {
        public string ToEmail { get; set; } = default!;

        public string Subject { get; set; } = default!;

        public string Body { get; set; } = default!;
    }
}
