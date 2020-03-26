using System;
using System.Collections.Generic;
using System.Linq;
using MailService;
using Xunit;

namespace MailServiceTests
{
    public class Tests
    {
        private static string testDbPath = "EmailRegisterTest.db";
        [Fact]
        public async void Test_SendMail_Returns_Int()
        {
            //arrange
            var ms = new MailClient(testDbPath);
            var expectedResult = 0;

            var email = new Email()
            {
                To = "dpalyov@visteon.com",
                From = "test@visteon.com",
                UseHtmlBody = false,
                UseStaticHtml = false,
                Subject = "Test sub",
                MessageBody = "Test body",
                Attachments = new List<string>
                {
                    "mailServiceConfig.json"
                }
            };

            ms.ConfigureMessage(email);
            //actual
            var actualResponse = await ms.SendMessageAsync();
            //Assert
            Assert.Equal(expectedResult, actualResponse);
        }

        [Fact]
        public void Test_SendMessageSync_Returs_Int()
        {

            var ms = new MailClient(testDbPath);
            var expectedResult = 0;

            var email = new Email()
            {
                To = "dpalyov@visteon.com",
                From = "test@visteon.com",
                UseHtmlBody = false,
                UseStaticHtml = false,
                Subject = "Test sub",
                MessageBody = "Test body"
            };

            ms.ConfigureMessage(email);
            //actual
            var actualResponse = ms.SendMessage();
            //Assert
            Assert.Equal(expectedResult, actualResponse);


        }

        [Fact]
        public void Test_RegisterEmail()
        {
            var email = new ScheduledEmail()
            {
                ProjectNumber = 99999,
                Application = "Test app",
                To = "dpalyov33@visteon.com",
                From = "test@visteon.com",
                UseHtmlBody = false,
                UseStaticHtml = false,
                Subject = "Test sub",
                MessageBody = "Test body",
                CreationDate = DateTime.Now,
                ReminderInterval = 0.05
            };

            var ms = new MailClient(testDbPath);
            var actual = ms.RegisterEmail(email);

            Assert.Equal(1, actual);
        }

        [Fact]
        public void Test_RegisterEmails()
        {
            var email1 = new ScheduledEmail()
            {
                ProjectNumber = 99999,
                Application = "Test app",
                To = "dpalyov@visteon.com",
                From = "test@visteon.com",
                UseHtmlBody = false,
                UseStaticHtml = false,
                Subject = "Test sub",
                MessageBody = "Test body",
                CreationDate = DateTime.Now,
                ReminderInterval = 0.05
            };

            var email2 = new ScheduledEmail()
            {
                ProjectNumber = 99999,
                Application = "Test app",
                To = "dpalyov@visteon.com",
                From = "random@visteon.com",
                UseHtmlBody = true,
                UseStaticHtml = false,
                Subject = "Test sub",
                MessageBody = "<span style='background-color:red'>Random red text</span>",
                CreationDate = DateTime.Now,
                ReminderInterval = 0.05
            };

            var arr = new ScheduledEmail[2] { email1, email2 };

            var ms = new MailClient(testDbPath);
            var actual = ms.RegisterEmails(arr);

            Assert.Equal(2, actual);
        }

        [Fact]
        public void Test_UnregisterEmail()
        {

            var ms = new MailClient(testDbPath);
            var actual = ms.UnregisterEmail(1);

            Assert.Equal(1, actual);
        }

        [Fact]
        public void Test_ReadEmail()
        {

            var expected = new ScheduledEmail()
            {
                ProjectNumber = 99999,
                Application = "Test app",
                To = "dpalyov@visteon.com",
                From = "test@visteon.com",
                UseHtmlBody = false,
                UseStaticHtml = false,
                Subject = "Test sub",
                MessageBody = "Test body",
                CreationDate = DateTime.Now,
                ReminderInterval = 0.05
            };

            var ms = new MailClient(testDbPath);
            var actual = ms.ReadEmail(6);

            Assert.Equal(expected.From, actual.From);
            Assert.Equal(expected.To, actual.To);
            Assert.Equal(expected.MessageBody, actual.MessageBody);
        }


        [Fact]
        public void Test_ReadEmails()
        {

            var ms = new MailClient(testDbPath);
            var actual = ms.ReadEmails(x => x.From.Equals("test@visteon.com"));

            Assert.True(actual.Count() == 1);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(3)]
        public void Test_UpdateEmails(int id)
        {

            var ms = new MailClient(testDbPath);
            var toUpdate = ms.ReadEmail(id);


            if (id == 6)
            {

                toUpdate.From = "test2@gmail.com";
                var actual = ms.UpdateEmail(toUpdate);
                Assert.True(actual);
            }
            else
            {
                Assert.Null(toUpdate);
            }

        }

        [Fact]
        public void Test_EmptyCollection()
        {
            var ms = new MailClient(testDbPath);
            var actual = ms.EmptyCollection();
            var collection = ms.ReadEmails();

            Assert.Empty(collection);
        }


    }
}
