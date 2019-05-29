using System.IO;
using System.Text;

namespace MailService
{
    public static class SystemLogger
    {
        public async static void Log(string message)
        {
            try
            {
                using (var sr = new StreamWriter("Log.txt", true, Encoding.UTF8,2400))
                {
                    await sr.WriteAsync(message + "\n");
                }
            }
            catch(IOException)
            {
                
            }
          
        }
    }
}
