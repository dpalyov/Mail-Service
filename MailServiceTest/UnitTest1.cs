using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MailService;

namespace MailServiceTest
{
    [TestClass]
    public class MailServiceUT
    {
        [TestMethod]
        public void TestSmtpConfigurator()
        {
            //arrange
            var host = "test";

            //act 
            service.ConfigureClient(host);
        }
    }
}
