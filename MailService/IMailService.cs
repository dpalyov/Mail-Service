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
        int RegisterEmail(Email email);
        int RegisterEmails(IEnumerable<Email> collection);
        int UnregisterEmail(int id);
        Email ReadEmail(int id);
        IEnumerable<Email> ReadEmails(Expression<Func<Email, bool>> predicate);
        IEnumerable<Email> ReadEmails();

        int EmptyCollection();
        bool IsDue(DateTime lastNotificationDate, double interval);
        bool UpdateEmail(Email email);

    }
}
