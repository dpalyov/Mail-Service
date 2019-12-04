using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace MailService
{
    public class SystemLogger
    {
        private readonly string _path;
        public SystemLogger(string path = "")
        {
            if (path.Equals(""))
            {
                _path = Directory.GetCurrentDirectory();
            }
            else
            {
                _path = path;
            }
        }
        public async void Log(string message)
        {

            Directory.CreateDirectory(_path);

            var file = Path.Combine(_path, "MailServiceLog.txt");
            var stream =  new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.Write, 2048, true);
            var cancellationToken = new CancellationTokenSource();
            var token = cancellationToken.Token;

            try
            {
                await stream.WriteAsync(Encoding.UTF8.GetBytes(message + "\n"), token);
            }
            catch (IOException ex)
            {
                await stream.WriteAsync(Encoding.UTF8.GetBytes(ex.Message + "\n"), token);
            }
            finally
            {
                stream.Dispose();
                cancellationToken.Dispose();
            }

        }
    }
}
