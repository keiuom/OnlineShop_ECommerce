using BuyNow.Core;

namespace Order.Core.Domain
{
    public class EmailMessage : BaseEntity<int>
    {
        public string Recipient { get; set; } = default!;

        public string Subject { get; set; } = default!;

        public string Body { get; set; } = default!;

        public bool IsSent { get; set; }

        public DateTime? SentAt { get; set; }

        public int SentCount { get; set; }
    }

}
