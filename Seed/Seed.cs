using System;
using MailService;

namespace Seed
{
    class Seed
    {
        static void Main(string[] args)
        {
            var ms = new MailClient();

            var emailCollection = new Email[4]
            {
                new Email
                {
                    Id = 4,
                    From = "dimitar@visteon.com",
                    To = "dpalyov@visteon.com",
                    MessageBody = "<span style='background-color:green'>Green span</span>",
                    UseHtmlBody = true,
                    UseStaticHtml = false,
                    CreationDate = DateTime.Now,
                    ReminderInterval = 0.05,

                },
                  new Email
                {
                    Id = 5,
                    From = "random@visteon.com",
                    To = "dpalyov@visteon.com",
                    MessageBody = "<span style='background-color:red'>Red span</span>",
                    UseHtmlBody = true,
                    UseStaticHtml = false,
                    CreationDate = DateTime.Now,
                    ReminderInterval = 0.25,

                },
                   new Email
                {
                    Id = 6,
                    From = "random@visteon.com",
                    To = "dpalyov@visteon.com",
                    MessageBody = "<span style='background-color:blue'>Blue span</span>",
                    UseHtmlBody = true,
                    UseStaticHtml = false,
                    CreationDate = DateTime.Now,
                    ReminderInterval = 0.5,

                },
                    new Email
                {
                    Id = 7,
                    From = "random2@visteon.com",
                    To = "dpalyov@visteon.com",
                    MessageBody = "<span style='background-color:orange'>Orange span</span>",
                    UseHtmlBody = true,
                    UseStaticHtml = false,
                    CreationDate = DateTime.Now,
                    ReminderInterval = 0.75,

                }
            };

            var resultCount = ms.RegisterEmails(emailCollection);

            Console.WriteLine(resultCount);

        }

        // private IEnumerable<Email> ReadJsonFile(string path)
        // {
        //     var file = new FileInfo(path);
        //     var jsonData = String.Empty;
        //     using (var str = file.OpenRead())
        //     {
        //         var byteArr = new byte[2048];
        //         var readBytes = str.Read(byteArr, 0, byteArr.Length);
        //         jsonData += Encoding.UTF8.GetString(byteArr);

        //     }

        //     Console.WriteLine(jsonData);

        //     var collectionResult = JsonSerializer.Deserialize<IEnumerable<Email>>(jsonData);
        // }
    }
}
