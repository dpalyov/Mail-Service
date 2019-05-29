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
            var port = 0;
            var useDefaultCredentials = true;

            //act
            var ms = new MailService.MailService();
            ms.ConfigureClient(host,port,useDefaultCredentials);

            //assert
            Assert.Equal(host, ms.Client.Host);
            Assert.True(ms.Client.UseDefaultCredentials);
            Assert.NotEqual(port, ms.Client.Port);
        }

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
            var ms = new MailService.MailService();
            ms.ConfigureMessage(subject,body,to, from, useHtmlBody);

            //assert
            Assert.Equal(subject,ms.Mail.Subject);
            Assert.Equal(body,ms.Mail.Body);
            Assert.Equal(useHtmlBody,ms.Mail.IsBodyHtml);
            Assert.True(ms.Mail.IsBodyHtml);
            Assert.Equal(to, ms.Mail.To[0].Address);
            Assert.Equal(from, ms.Mail.From.Address);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(-3)]
        [InlineData(130)]
        [InlineData(25)]
        public void TestSendMailWithWrongResponse(int responseCode)
        {
            //arrange
            
            var ms = new MailService.MailService();
            ms.ConfigureClient();
            ms.ConfigureMessage("Test", "test msg");
           

            //actual
            var actualResponse = ms.SendMessage();

            //Assert
            Assert.NotEqual(responseCode, actualResponse);
        }

        [Fact]
        public void TestSendMailWithSuccessResponse()
        {
            //arrange
            var ms = new MailService.MailService();
            ms.ConfigureClient();
            ms.ConfigureMessage("Test", "test msg");
            var expectedResult = 0;


            //actual
            var actualResponse = ms.SendMessage();

            //Assert
            Assert.Equal(expectedResult, actualResponse);
        }
    }
}
