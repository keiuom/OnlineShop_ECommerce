using Microsoft.Extensions.Options;
using Order.Core.Mails;

namespace Order.Services.Mails
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(IOptions<SmtpSettings> smtpSettingsOption)
        {
            _smtpSettings = smtpSettingsOption.Value;
        }

        public async Task SendAsync(string recipient, string subject, string body)
        {

        }
    }
}
