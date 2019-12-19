using System.Net.Mail;
using System.Net;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using LiteDB;

namespace MailService
{
    public class MailClient : IMailService
    {
        private readonly MailMessage _mail;
        private readonly SmtpClient _client;
        private readonly Configuration _config;
        private readonly ILogger<MailClient> _logger;



        public MailClient(ILogger<MailClient> logger)
        {
            _mail = new MailMessage();
            _client = new SmtpClient();
            _logger = logger;
         
        }


        public MailClient(bool useConfigFile, ILogger<MailClient> logger)
        {
            _mail = new MailMessage();
            _client = new SmtpClient();
            _logger = logger;

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

        public void ConfigureClient(string host, int port = 25, bool defaultCredentials = true, ICredentialsByHost credentials = null)
        {

            try
            {
                _client.Host = host;
                _client.Port = port;
                _client.UseDefaultCredentials = defaultCredentials;

                if (!defaultCredentials && credentials != null)
                {
                    _client.Credentials = credentials;
                }

                if (!defaultCredentials && credentials == null)
                {
                    throw new NullReferenceException("Must provide valid credentials...");
                }

                _client.DeliveryMethod = SmtpDeliveryMethod.Network;
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex.Message);
            }


        }

        public void ConfigureClient()
        {
            _client.Host = _config.SmtpConfiguration.Host;
            _client.Port = _config.SmtpConfiguration.Port;
            _client.UseDefaultCredentials = _config.SmtpConfiguration.UseDefaultCredentials;


            try
            {

                if (!_client.UseDefaultCredentials && _config.SmtpConfiguration.Credentials.Username != "" && _config.SmtpConfiguration.Credentials.Password != "")
                {
                    var username = _config.SmtpConfiguration.Credentials.Username;
                    var password = _config.SmtpConfiguration.Credentials.Password;
                    _client.Credentials = new NetworkCredential(username, password);
                }



                if (!_client.UseDefaultCredentials && (_config.SmtpConfiguration.Credentials.Username != "" || _config.SmtpConfiguration.Credentials.Password != ""))
                {
                    throw new NullReferenceException("Must provide valid credentials...");
                }

            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex.Message);
            }

            _client.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        public void ConfigureMessage(
            string to,
            string from,
            string subject = "",
            string body = "",
            bool htmEnabledBody = false)
        {

            var recepientsList = to.Split(";");

            foreach (var recepient in recepientsList)
            {
                _mail.To.Add(recepient);
            }

            _mail.From = new MailAddress(from);
            _mail.Body = body;
            _mail.IsBodyHtml = htmEnabledBody;
            _mail.Subject = subject;


        }
        public void ConfigureMessage()
        {
            var recepients = _config.MailConfiguration.Recepients;
            var recepientsList = recepients.Split(";");

            foreach (var recepient in recepientsList)
            {
                _mail.To.Add(recepient);
            }

            var sender = _config.MailConfiguration.Sender;
            _mail.From = new MailAddress(sender);
            _mail.Body = _config.MailConfiguration.Message;
            _mail.IsBodyHtml = _config.MailConfiguration.UseHtmlBody;
            _mail.Subject = _config.MailConfiguration.Subject;
        }


        public int SendMessage(Dictionary<string, string> opts = null)
        {

            object userToken = null;

            // SendCompletedEventHandler handler = delegate (object s, AsyncCompletedEventArgs e)
            // {

            // };


            try
            {
                _client.SendCompleted += new SendCompletedEventHandler(SendCompletedFeedback);
                _client.SendAsync(_mail, userToken);


                if (opts != null)
                {
                    foreach (var kv in opts)
                    {
                        _logger.LogInformation($"{kv.Key}: {kv.Value}");
                    }

                }

                return 0;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.GetType().ToString() + ":" + ex.Message);
                return 1;
            }
        }

        public int SendMessageSync(Dictionary<string, string> opts = null)
        {
            try
            {
                _client.SendCompleted += new SendCompletedEventHandler(SendCompletedFeedback);
                _client.Send(_mail);
                _logger.LogInformation($"Sent successfully on {DateTime.Now}");

                if (opts != null)
                {
                    foreach (var kv in opts)
                    {
                        _logger.LogInformation($"{kv.Key}: {kv.Value}");
                    }

                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.GetType().ToString() + ":" + ex.Message);
                return 1;
            }
        }

        private  void SendCompletedFeedback(object s, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                _logger.LogWarning("Message cancelled...");
            }
            else if (e.Error != null)
            {
                _logger.LogError(e.Error.GetType().ToString() + ":" + e.Error.Message);
            }
            else
            {
                _logger.LogInformation($"Sent successfully on {DateTime.Now}");

            }

            //_mail.Dispose();
            //_client.Dispose();
        }

        public int RegisterEmail(Email email)
        {
            using(var db = new LiteDatabase("EmailRegister.db"))
            {
                var emailCollection = db.GetCollection<Email>("emails");

               var doc = emailCollection.Insert(email);
                emailCollection.EnsureIndex(x => x.Id);

               if(doc != null)
               {
                   return 1;
               }

            }

            return 0;
        }

      
    }
}
