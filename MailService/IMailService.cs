using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MailService
{
    public interface IMailService
    {
    //    void ConfigureClient(string host , int port = 0, bool defaultCredentials = true, ICredentialsByHost credentials = null);
    //    void ConfigureClient();
        void ConfigureMessage(Email email);
        Task<int> SendMessageAsync(Dictionary<string, string> opts = null);
        int SendMessage(Dictionary<string, string> opts = null);
        int RegisterEmail(ScheduledEmail email);
        int RegisterEmails(IEnumerable<ScheduledEmail> collection);
        int UnregisterEmail(int id);
        ScheduledEmail ReadEmail(int id);
        IEnumerable<ScheduledEmail> ReadEmails(Expression<Func<ScheduledEmail, bool>> predicate);
        IEnumerable<ScheduledEmail> ReadEmails();

        int EmptyCollection();
        bool IsDue(DateTime lastNotificationDate, double interval);
        bool UpdateEmail(ScheduledEmail email);

    }
}
