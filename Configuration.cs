
namespace MailService
{
    public class Configuration
    {
        public SmtpConfiguration SmtpConfiguration { get; set; }
        public MailConfiguration MailConfiguration { get; set; }
    }

    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class SmtpConfiguration
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public Credentials Credentials { get; set; }
    }

    public class MailConfiguration
    {
        public string Recepients { get; set; }
        public string Sender { get; set; }
        public bool UseHtmlBody { get; set; }
    }
}
