using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;

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

            var loginInput = GetElementsByTagName("input").Where(w => w.GetName() == "email").First();
            if (string.IsNullOrWhiteSpace(loginInput.GetValue()))
            {
                loginInput.SendKeys(login);
            }

            GetElementsByTagName("button").Where(w => w.GetAriaLabel() == "Вперед").First().ToClick(2000);

            var passwordInput = GetElementsByTagName("input").Where(w => w.GetName() == "password").First();
            if (string.IsNullOrWhiteSpace(passwordInput.GetValue()))
            {
                passwordInput.SendKeys(password);
            }

            GetElementsByTagName("button").Where(w => w.GetAriaLabel() == "Войти").First().ToClick(2000);

            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void Reblog()
        {
            SwitchToFrame(GetElementByXPath("/html/body/iframe[1]"));
            GetElementByClassName("reblog-button").ToClick(1500);
            ExecuteScript("document.getElementsByClassName('post-form--form')[0].remove();");
            GetElementByClassName("create_post_button").ToClick(1500);
        }
        /// <inheritdoc/>
        public void LikePost()
        {
            SwitchToFrame(GetElementByXPath("/html/body/iframe[1]"));

            var button = GetElementByClassName("like-button");
            if (!button.Displayed)
            {
                return;
            }

            button.ToClick(1500);
        }
        /// <inheritdoc/>
        public void RemoveLike()
        {
            SwitchToFrame(GetElementByXPath("/html/body/iframe[1]"));

            var button = GetElementByClassName("unlike-button");
            if (!button.Displayed)
            {
                return;
            }

            button.ToClick(1500);
        }
        /// <inheritdoc/>
        public void SubscribeToBlog()
        {
            GetElementByTagName("form").FindElements(SearchMethod.Tag, "button")
                .Where(w => w.GetInnerText() == "Читать").FirstOrDefault()?.ToClick(2000);
        }
        /// <inheritdoc/>
        public void UnsubscribeToBlog()
        {
            GetElementByTagName("form").FindElements(SearchMethod.Tag, "button")
                .Where(w => w.GetInnerText() == "Не читать").FirstOrDefault()?.ToClick(2000);
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
