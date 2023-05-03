namespace Order.Common.Models
{
    public class QueueEmailModel
    {
        public string Recipient { get; set; } = default!;

        public string Subject { get; set; } = default!;

        public string Body { get; set; } = default!;
    }
}
