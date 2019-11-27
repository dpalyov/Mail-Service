using System.Net.Mail;
using System.Net;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MailService
{
    public class MailClient : IMailService
    {
        private readonly MailMessage _mail;
        private readonly SmtpClient _client;
        private readonly Configuration _config;

        public MailMessage Mail { get => _mail; private set { } }
        public SmtpClient Client { get => _client; private set { } }

        public MailClient()
        {
            _mail = new MailMessage();
            _client = new SmtpClient();
        }
       

        public MailClient(bool useConfigFile)
        {
            _mail = new MailMessage();
            _client = new SmtpClient();

            if (useConfigFile)
            {
                using (var reader = new StreamReader("config.json"))
                {
                    var config = reader.ReadToEnd();
                    var configModel = JsonConvert.DeserializeObject<Configuration>(config);
                    _config = configModel;
                }
            }
        }

        public void ConfigureClient(string host = null, int port = 0, bool? defaultCredentials = null, ICredentialsByHost credentials = null)
        {

            try
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
            catch (NullReferenceException)
            {
                SystemLogger.Log("Possibly Configuration object is not properly configured. If you are not using a json configuration, please provide the arguments to the ConfigurationClient method");
            }

        }

        public void ConfigureMessage(
            string subject = "",
            string body = "",
            string to = null, 
            string from = null, 
            bool htmEnabledBody = false)
        {
            try
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
            catch(NullReferenceException)
            {
                SystemLogger.Log("Possibly Configuration object is not properly configured. If you are not using a json configuration, please provide the arguments through the public properties of the Configuration object");
            }
           
        }


        public int SendMessage(Dictionary<string,string> opts = null)
        {
            try
            {
                //SystemLogger.Log("Sending...");
                _client.Send(_mail);
                SystemLogger.Log($"Sent successfully on {DateTime.Now}");

                
                if(opts != null)
                {
                    foreach (var kv in opts)
                    {
                        SystemLogger.Log($"{kv.Key}: {kv.Value}");
                    }

                }


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
