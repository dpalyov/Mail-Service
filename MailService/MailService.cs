using System.Net.Mail;
using System.Net;
using System;
using System.IO;
using Newtonsoft.Json;

namespace MailService
{
    public class MailService
    {
        private readonly MailMessage _mail;
        private readonly SmtpClient _client;
        private readonly Configuration _config;

        public MailMessage Mail { get => _mail; private set { } }
        public SmtpClient Client { get => _client; private set { } }
       

        public MailService()
        {
            _mail = new MailMessage();
            _client = new SmtpClient();

            using (var reader = new StreamReader("config.json"))
            {
                var config = reader.ReadToEnd();
                var configModel = JsonConvert.DeserializeObject<Configuration>(config);
                _config = configModel;
            }


        }

        public void ConfigureClient(string host = null, int port = 0, bool? defaultCredentials = null, ICredentialsByHost credentials = null)
        {
            _client.Host = host ?? _config.SmtpConfiguration.Host;
            
            port = port == 0 ? _config.SmtpConfiguration.Port : port;

            if(port != 0)
            {
                _client.Port = port;
            }

            _client.UseDefaultCredentials = defaultCredentials ?? _config.SmtpConfiguration.UseDefaultCredentials;

            if (!_client.UseDefaultCredentials)
            {
                var username = _config.SmtpConfiguration.Credentials.Username;
                var password = _config.SmtpConfiguration.Credentials.Password;
                _client.Credentials = credentials ?? new NetworkCredential(username, password);
            }
             
            _client.DeliveryMethod = SmtpDeliveryMethod.Network;
            
            }

        public void ConfigureMessage(
            string subject = "",
            string body = "",
            string to = null, 
            string from = null, 
            bool htmEnabledBody = false)
        {

            var recepients = to ?? _config.MailConfiguration.Recepients;
            var recepientsList = recepients.Split(";");

            foreach (var recepient in recepientsList)
            {
                _mail.To.Add(recepient);
            }

            var sender = from ?? _config.MailConfiguration.Sender;
            _mail.From = new MailAddress(sender);
            _mail.Body = body;
            _mail.IsBodyHtml = htmEnabledBody ? htmEnabledBody : _config.MailConfiguration.UseHtmlBody;
            _mail.Subject = subject;
        }


        public int SendMessage()
        {
            try
            {
                SystemLogger.Log("Sending...");
                _client.Send(_mail);
                SystemLogger.Log($"Sent successfully on {DateTime.Now}");
                return 0;
            }
            catch(SmtpException ex)
            {
                SystemLogger.Log(ex.Message);
                return 1;
            }
        }
    }
}
