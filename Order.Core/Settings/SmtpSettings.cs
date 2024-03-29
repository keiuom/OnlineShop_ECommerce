﻿namespace Order.Core.Settings
{
    public class SmtpSettings
    {
        public string Server { get; set; } = default!;

        public int Port { get; set; }

        public string SenderName { get; set; } = default!;

        public string SenderEmail { get; set; } = default!;

        public string UserName { get; set; } = default!;

        public string Password { get; set; } = default!;
    }
}
