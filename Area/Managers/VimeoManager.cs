using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class VimeoManager : BrowserManager, IVimeoManager
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            OpenPageInNewTab("https://vimeo.com/");

            if (GetElementById("topnav_menu_avatar") != null)
            {
                CloseTab();
                SwitchToTab();
                return;
            }


            GetElementById("nav-cta-login").Click();

            var inputLogin = GetElementById("signup_email");
            if (string.IsNullOrWhiteSpace(inputLogin.GetValue()))
            {
                inputLogin.SendKeys(login);
            }

            var inputPassword = GetElementById("login_password");
            if (string.IsNullOrWhiteSpace(inputPassword.GetValue()))
            {
                inputPassword.SendKeys(password);
            }

            GetElementByClassName("js-email-submit").Click();
            Thread.Sleep(1500);

            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
