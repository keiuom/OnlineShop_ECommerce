using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Order.Core.Settings;

namespace Order.Services.Mails
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(IOptions<SmtpSettings> smtpSettingsOption)
        {
            _smtpSettings = smtpSettingsOption.Value;
        }

        public async Task SendAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpSettings.UserName, _smtpSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();

            builder.HtmlBody = mailRequest.Body;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = mailRequest.Body };
            using var smtp = new SmtpClient();
            smtp.Connect(_smtpSettings.Server, _smtpSettings.Port, true);
            smtp.AuthenticationMechanisms.Remove("XOAUTH2");
            smtp.Authenticate(_smtpSettings.UserName, _smtpSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
