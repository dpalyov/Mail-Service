
namespace MailService
{

    public class Configuration
    {

        public Logging Logging {get; set;}
        public SMTP SMTP { get; set; }
    }

    public class Logging 
    {
        public LogLevel LogLevel { get; set; }
    }

    public class LogLevel 
    {
        public string Default { get; set; }
    }

    public class Credentials
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class SMTP
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public Credentials Credentials { get; set; }

        public string RegisterPath { get; set; }
    }


}
