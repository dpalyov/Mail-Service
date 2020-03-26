using System;
using System.Collections.Generic;

namespace MailService
{
    public class Email
    {

        public Email()
        {
            Attachments = new List<string>();
        }

        public string From { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string MessageBody { get; set; }
        public bool UseStaticHtml { get; set; }
        public bool UseHtmlBody { get; set; }
        public IEnumerable<string> Attachments { get; set; }

        public override string ToString()
        {
            base.ToString();
            return $@"From: {From}
                      To: {To}
                      Subject: {Subject}";
        }

    }
}