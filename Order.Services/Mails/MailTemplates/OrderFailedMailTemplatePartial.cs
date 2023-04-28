namespace Order.Services.Mails.MailTemplates
{
    public partial class OrderFailedMailTemplate
    {
        private string Reason { get; set; } = default!;

        private string SupportUserEmail { get; set; } = default!;

        public OrderFailedMailTemplate(string reason, string supportUserEmail)
        {
            Reason = reason;
            SupportUserEmail = supportUserEmail;
        }
    }
}
