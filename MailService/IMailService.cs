using System.Collections.Generic;
using System.Net;


namespace MailService
{
    public interface IMailService
    {
       void ConfigureClient(string host , int port = 0, bool defaultCredentials = true, ICredentialsByHost credentials = null);
       void ConfigureClient();
        void ConfigureMessage(
            string to,
             string from,
             string subject = "",
             string body = "",
             bool htmEnabledBody = false);
        void ConfigureMessage();
        int SendMessage(Dictionary<string,string> opts = null);

    }
}
