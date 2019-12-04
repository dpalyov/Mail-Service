using MailService;
using System;
using System.IO;
using Xunit;

namespace MailServiceTests
{
    public class Tests
    {
        [Fact]
        public void TestSmtpConfig()
        {
            //arrange
            var host = "test";

            //act
            var ms = new MailClient(true, Directory.GetCurrentDirectory());
            ms.ConfigureClient(host);

            //assert
            Assert.Equal(host, ms.Client.Host);
            Assert.True(ms.Client.UseDefaultCredentials);
            Assert.Equal(25, ms.Client.Port);
        }

        //[Fact]
        //public void TestSmtpConfig_Throws_NullReferenceException()
        //{
        //    //arrange
        //    var host = "test";

        //    //act
        //    var ms = new MailClient(true, Directory.GetCurrentDirectory());
        //    ms.ConfigureClient(host,25,false);

        //    //assert
        //    Assert.Throws<NullReferenceException>(() => ms.ConfigureClient(host));
        //}
        //[Fact]
        //public void TestSmtpConfigNoArgs_Throws_NullReferenceException()
        //{
        //    //arrange
        //    //act

        //    var ms = new MailClient(true, Directory.GetCurrentDirectory());
        //    ms.ConfigureClient();

        //    //assert
        //    Assert.Throws<NullReferenceException>(() => ms.ConfigureClient());
        //}

        [Fact]
        public void TestMailConfig()
        {
            //arrange
            var subject = "test";
            var body = "body test";
            var useHtmlBody = true;
            var to = "recepient@visteon.com";
            var from = "sender@visteon.com";

            //act
            var ms = new MailClient(true, Directory.GetCurrentDirectory());
            ms.ConfigureMessage(to,from,subject,body,useHtmlBody);

            //assert
            Assert.Equal(subject,ms.Mail.Subject);
            Assert.Equal(body,ms.Mail.Body);
            Assert.Equal(useHtmlBody,ms.Mail.IsBodyHtml);
            Assert.True(ms.Mail.IsBodyHtml);
            Assert.Equal(to, ms.Mail.To[0].Address);
            Assert.Equal(from, ms.Mail.From.Address);
        }

        [Fact]
        public void TestSendMailWithSuccessResponse()
        {
            var loggerPath = Directory.GetCurrentDirectory();
            //arrange
            var ms = new MailClient(true, loggerPath);
            ms.ConfigureClient();
            ms.ConfigureMessage();
            var expectedResult = 0;

            //actual
            var actualResponse = ms.SendMessage();

            //Assert
            Assert.Equal(expectedResult, actualResponse);
        }

        [Fact]
        public void TestLog_Returns_Void()
        {

            var path = Directory.GetCurrentDirectory();
            var logger = new SystemLogger(path);
            var testMsg = "Test message";

            logger.Log("Test message");

            var logFile = Path.Combine(path, "MailServiceLog.txt");
            var log = File.ReadAllLines(logFile);

            Assert.True(File.Exists(logFile));
            Assert.Equal(log[log.Length - 1], testMsg);


        }
    }
}
