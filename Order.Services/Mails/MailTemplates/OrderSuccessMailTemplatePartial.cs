namespace Order.Services.Mails.MailTemplates
{
    public partial class OrderSuccessMailTemplate
    {
        private int OrderId { get; set; } = default!;

        private string SupportUserEmail { get; set; } = default!;

        public OrderSuccessMailTemplate(int orderId, string supportUserEmail)
        {
            OrderId = orderId;
            SupportUserEmail = supportUserEmail;
        }
    }
}
