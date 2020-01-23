using System.Net.Mail;
using System.Net;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using LiteDB;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using Serilog;
using System.Threading.Tasks;

namespace MailService
{
    public class MailClient : IMailService, IDisposable
    {
        private readonly MailMessage _mail;
        private readonly SmtpClient _client;
        private readonly ILogger<MailClient> _logger;
        private readonly string _registerPath;

        private static string registerPath = "C:\\Users\\" + Environment.GetEnvironmentVariable("USERNAME") + "\\AppData\\Local\\EmailRegister.db";
        private static IConfigurationRoot jsonConfig = new ConfigurationBuilder()
        .AddJsonFile("mailServiceConfig.json", true, false)
        .Build();
        private static Configuration _config = jsonConfig
        .GetSection("Configuration")
        .Get<Configuration>();



        public MailClient() : this(_config.SMTP.Host, _config.SMTP.Port, null, registerPath)
        {
            var serilog = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine("Logs", "app.log"), rollingInterval: RollingInterval.Day)
            .CreateLogger();

            var lf = new LoggerFactory();
            var sl = SerilogLoggerFactoryExtensions.AddSerilog(lf, serilog, true);

            _logger = sl.CreateLogger<MailClient>();
        }
        public MailClient(ILogger<MailClient> logger) : this(_config.SMTP.Host, _config.SMTP.Port, logger, registerPath)
        {

        }

        public MailClient(string registerPath) : this(_config.SMTP.Host, _config.SMTP.Port, null, registerPath)
        {
            var serilog = new LoggerConfiguration()
                       .MinimumLevel.Debug()
                       .WriteTo.Console()
                       .WriteTo.File(Path.Combine("Logs", "app.log"), rollingInterval: RollingInterval.Day)
                       .CreateLogger();

            var lf = new LoggerFactory();
            var sl = SerilogLoggerFactoryExtensions.AddSerilog(lf, serilog, true);

            _logger = sl.CreateLogger<MailClient>();
        }

        public MailClient(string host, int port, ILogger<MailClient> logger, string registerPath)
        {

            _client = new SmtpClient(host, port);
            _client.UseDefaultCredentials = _config.SMTP.UseDefaultCredentials;

            if (!_config.SMTP.UseDefaultCredentials)
            {
                _client.Credentials = new NetworkCredential(_config.SMTP.Credentials.Username, _config.SMTP.Credentials.Password);
            }
            _client.DeliveryMethod = SmtpDeliveryMethod.Network;

            if (!_client.UseDefaultCredentials
            && (_config.SMTP.Credentials.Username == String.Empty || _config.SMTP.Credentials.Password == String.Empty))
            {
                throw new ArgumentException("Username or Password argument(s) missing when Default credentials is set to false");
            }

            _client.SendCompleted += new SendCompletedEventHandler(SendCompletedFeedback);

            _mail = new MailMessage();
            _logger = logger;
            _registerPath = registerPath;

        }

        /// <summary>
        /// Configures the message to be sent - e.g. from, to, subject, message etc.
        /// </summary>
        /// <param name="email">The Email model to be passed</param>
        public void ConfigureMessage(Email email)
        {

            var recepientsList = email.To.Split(";", StringSplitOptions.RemoveEmptyEntries);

            _mail.To.Clear();
            foreach (var recepient in recepientsList)
            {
                _mail.To.Add(recepient);
            }

            _mail.From = new MailAddress(email.From);
            _mail.Body = email.MessageBody;
            _mail.IsBodyHtml = email.UseHtmlBody;
            _mail.Subject = email.Subject;


        }


        /// <summary>
        /// Send an async request to the smpt server with the configured message.
        /// </summary>
        /// <param name="opts">Logging opts to be used when the message gets sent</param>
        /// <returns></returns>
        public async Task<int> SendMessageAsync(Dictionary<string, string> opts = null)
        {

            try
            {
                // _client.SendCompleted += new SendCompletedEventHandler(SendCompletedFeedback);
                await _client.SendMailAsync(_mail);


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
        /// <summary>
        /// Sends a sync request to the smtp server.
        /// </summary>
        /// <param name="opts">Logging options to be logged once message is sent</param>
        /// <returns></returns>
        public int SendMessage(Dictionary<string, string> opts = null)
        {
            try
            {
                // _client.SendCompleted += new SendCompletedEventHandler(SendCompletedFeedback);
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
        /// <summary>
        /// Callback function that triggers on sending event.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void SendCompletedFeedback(object s, AsyncCompletedEventArgs e)
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


        }
        /// <summary>
        /// Registers a mail to the Email registry.
        /// </summary>
        /// <param name="email">Email model</param>
        /// <returns></returns>
        public int RegisterEmail(Email email)
        {
            using (var db = new LiteDatabase(_registerPath))
            {
                var emailCollection = db.GetCollection<Email>("emails");

                var docId = emailCollection.Insert(email);
                var docs = emailCollection.FindAll();
                emailCollection.EnsureIndex(x => x.Id);

                if (docId != null)
                {
                    return 1;
                }

            }

            return 0;
        }

        public int RegisterEmails(IEnumerable<Email> collection)
        {
            using (var db = new LiteDatabase(_registerPath))
            {
                var emailCollection = db.GetCollection<Email>("emails");

                var result = emailCollection.InsertBulk(collection);
                emailCollection.EnsureIndex(x => x.Id);

                return result;
            }
        }

        public int UnregisterEmail(int id)
        {
            using (var db = new LiteDatabase(_registerPath))
            {
                var emailCollection = db.GetCollection<Email>("emails");

                return emailCollection.Delete(x => x.Id == id);

            }

        }

        public Email ReadEmail(int id)
        {
            using (var db = new LiteDatabase(_registerPath))
            {
                var emailCollection = db.GetCollection<Email>("emails");

                var doc = emailCollection.FindById(id);

                return doc;

            }

        }


        public IEnumerable<Email> ReadEmails(Expression<Func<Email, bool>> predicate)
        {
            using (var db = new LiteDatabase(_registerPath))
            {
                var emailCollection = db.GetCollection<Email>("emails");

                var docs = emailCollection.Find(predicate);

                return docs;

            }

        }

        public IEnumerable<Email> ReadEmails()
        {

            _logger.LogInformation(Environment.CurrentDirectory);
            using (var db = new LiteDatabase(_registerPath))
            {
                var emailCollection = db.GetCollection<Email>("emails");

                var docs = emailCollection.FindAll();

                return docs;

            }

        }

        public bool UpdateEmail(Email email)
        {
            using (var db = new LiteDatabase(_registerPath))
            {
                var emailCollection = db.GetCollection<Email>("emails");

                var isUpdated = emailCollection.Update(email);

                return isUpdated;

            }

        }

        public int EmptyCollection()
        {
            using (var db = new LiteDatabase(_registerPath))
            {
                var collection = db.GetCollection<Email>("emails");
                return collection.Delete(x => x.Id > 0);

            }
        }

        public bool IsDue(DateTime lastNotificationDate, double interval)
        {
            var intervalTicks = TimeSpan.FromHours(interval).Ticks;
            var ticksLastNotifPlusInterval = lastNotificationDate.Ticks + intervalTicks;
            var ticksNow = DateTime.Now.Ticks;
            return ticksLastNotifPlusInterval <= ticksNow;
        }

        public void Dispose()
        {
            _mail.Dispose();
            _client.Dispose();
        }
    }
}
