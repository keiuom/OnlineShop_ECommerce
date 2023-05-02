using System.ComponentModel.DataAnnotations;

namespace Order.Common.Models
{
    public record AddEmailMessageModel([Required] string Recipient, [Required] string Subject, [Required] string Body);
}
