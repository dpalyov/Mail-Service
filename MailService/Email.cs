using System;
using System.ComponentModel.DataAnnotations;

namespace MailService
{
    public class Email
    {

        private  DateTime _creationDate;
        private  DateTime _lastNotificationDate;

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        public string From { get; set; }

        [Required]
        [MinLength(5)]
        public string To { get; set; }

        public string Subject { get; set; }

        public string MessageBody { get; set; }
        public bool UseStaticHtml { get; set; }

        public bool UseHtmlBody { get; set; }
        [Required]
        public DateTime CreationDate
        {
            get { return _creationDate; }
            set 
            {
                _creationDate = value;
                _lastNotificationDate = value;
            }
            
        }

        public DateTime LastNotificationDate 
        { 
            get {return _lastNotificationDate; }
            set { _lastNotificationDate = value; }
        }
        public double ReminderInterval { get; set; }

        public override string ToString()
        {
            base.ToString();
            return $@"From: {From}
                      To: {To}
                      Subject: {Subject}";
        }

    }
}