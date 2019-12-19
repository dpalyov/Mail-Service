using System;
using System.ComponentModel.DataAnnotations;

namespace MailService 
{
    public class Email 
    {
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
        public DateTime CreationDate { get; set; }

        public int ReminderInterval { get; set; }

    }
}