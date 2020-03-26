using System;
using System.Collections.Generic;

namespace MailService
{
    public class ScheduledEmail : Email
    {


        private DateTime _creationDate;

        public int Id { get; set; }

        public int ProjectNumber { get; set; }

        public string Application { get; set; }

        public string Type { get; set; }

        public DateTime CreationDate
        {
            get { return _creationDate; }
            set
            {
                _creationDate = value;
                LastNotificationDate = value;
            }

        }
        public DateTime LastNotificationDate { get; set; }
        public double ReminderInterval { get; set; }


    }
}