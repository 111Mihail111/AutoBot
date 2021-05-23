using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.Services;
using AutoBot.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace AUTOBOT.TESTS.Managers
{
    [TestClass]
    public class YouTubeManagerTest : TestConfiguration
    {
        private ChromeDriver _chromeDriver;
        private YouTubeManager _youTubeManager;
        private Account _accounts;
        private readonly string _testResultsfolder = "/YouTubeManagerTest/";

        [TestInitialize]
        public void Init()
        {
            GetAccounts();

            var option = new ChromeOptions
            {
                BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
            };

            option.AddArgument("--disable-notifications");
            option.AddArgument("--mute-audio");
            option.AddArgument("--start-maximized");

            _youTubeManager = new YouTubeManager();
            _youTubeManager.SetContextBrowserManager(_chromeDriver);

            _chromeDriver = new ChromeDriver(PATH_TO_DRIVER, option, TimeSpan.FromSeconds(200));

            _accounts = AccountService.GetAccounts().Where(w => w.AccountType == AccountType.YouTube).First();
        }

        [TestMethod]
        public void YouTube_Authorization_Return_Valid_url()
        {
            try
            {
                _youTubeManager.Authorization(_accounts.Login, _accounts.Password);

                Assert.AreEqual("https://www.youtube.com/account", _chromeDriver.Url);

                _chromeDriver.GetScreenshot();

                _chromeDriver.Quit();
                _chromeDriver.Close();
            }
            catch (Exception exception)
            {
                _chromeDriver.Quit();
                _chromeDriver.Close();
            }
        }

        private static void SaveTestResults(string testName)
        {

        }

        private static void GetAccounts()
        {
            string fileData = string.Empty;
            using (var stream = new StreamReader(PATH_TO_FILE_ACCOUNTS, Encoding.Default))
            {
                fileData = stream.ReadToEnd();
            };

            AccountManager accountManager = new();
            accountManager.SaveAccounts(fileData);
        }
    }
}