using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MailService
{
    public interface IMailService
    {
       void ConfigureClient(string host = null, int port = 0, bool? defaultCredentials = null, ICredentialsByHost credentials = null);
       void ConfigureMessage(
            string subject = "",
            string body = "",
            string to = null,
            string from = null,
            bool htmEnabledBody = false);
        int SendMessage(Dictionary<string,string> opts = null);

    }
}
