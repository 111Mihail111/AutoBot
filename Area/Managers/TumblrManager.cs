using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class TumblrManager : BrowserManager, ITumblr
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            string url = "https://www.tumblr.com/dashboard";
            OpenPageInNewTab(url);

            if (GetUrlPage() == url)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            var loginInput = GetElementById("signup_determine_email");
            if (string.IsNullOrWhiteSpace(loginInput.GetValue()))
            {
                loginInput.SendKeys(login);
            }

            GetElementById("signup_forms_submit").Click();
            Thread.Sleep(1500);

            GetElementById("signup_magiclink").FindElements(SearchMethod.Tag, "a")
                .Where(w => w.GetInnerText() == "Войти с паролем").First().Click();
            Thread.Sleep(500);

            var passwordInput = GetElementById("signup_password");
            if (string.IsNullOrWhiteSpace(passwordInput.GetValue()))
            {
                passwordInput.SendKeys(password);
            }

            GetElementById("signup_forms_submit").Click();
            Thread.Sleep(500);

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
