using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class YouTubeManager : BrowserManager, IYouTubeManager
    {
        public void Authorization(string login, string password)
        {
            string url = "https://www.youtube.com/";
            OpenPageInNewTab(url);

            if (GetElementById("avatar-btn") != null)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            ButtonLoginClick();

            var savedAccountDiv = GetElementById("profileIdentifier");
            if (savedAccountDiv != null)
            {
                savedAccountDiv.Click();
                AuthorizationUnderSavedProfile(password);
                return;
            }

            var inputLogin = GetElementById("identifierId");
            if (string.IsNullOrWhiteSpace(inputLogin.GetValue()))
            {
                inputLogin.SendKeys(login);
            }

            GetElementById("identifierNext").FindElement(SearchMethod.Tag, "button").Click();

            var inputPassword = GetElementById("password").FindElement(SearchMethod.Tag, "input");
            if (string.IsNullOrWhiteSpace(inputPassword.GetValue()))
            {
                inputPassword.SendKeys(password);
            }

            GetElementById("passwordNext").FindElement(SearchMethod.Tag, "button").Click();
            Thread.Sleep(2000);

            GoToUrl(url);
            ButtonLoginClick();

            CloseTab();
            SwitchToTab();
        }

        public void SubscribeToChannel()
        {
            GetElementById("channel-header-container")
                .FindElement(SearchMethod.Id, "subscribe-button").FindElement(SearchMethod.Tag, "paper-button").Click();
            Thread.Sleep(2000);
        }

        public void LikeUnderVideo()
        {
            GetElementById("top-level-buttons").FindElements(SearchMethod.Tag, "button").First().Click();
            Thread.Sleep(2000);
        }

        public void DislikeUnderVideo()
        {
            GetElementById("top-level-buttons").FindElements(SearchMethod.Tag, "button")
                .Where(w => w.GetAttribute("aria-pressed") == "false").Last().Click();
            Thread.Sleep(2000);
        }

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            GetDriver(chromeDriver);
        }

        /// <summary>
        /// Авторизация под сохраненным профилем
        /// </summary>
        /// <param name="password">Пароль</param>
        protected void AuthorizationUnderSavedProfile(string password)
        {
            var inputPass = GetElementById("password").FindElement(SearchMethod.Tag, "input");
            if (string.IsNullOrWhiteSpace(inputPass.GetValue()))
            {
                inputPass.SendKeys(password);
            }

            GetElementById("passwordNext").FindElement(SearchMethod.Tag, "button").Click();
            Thread.Sleep(2000);

            CloseTab();
            SwitchToTab();
        }
        /// <summary>
        /// Нажать на кнопку авторизации
        /// </summary>
        protected void ButtonLoginClick()
        {
            var divContainer = GetElementById("buttons").FindElements(SearchMethod.ClassName, "style-suggestive");
            foreach (var item in divContainer)
            {
                if (item.GetId() == "button")
                {
                    item.Click();
                    break;
                }
            }
        }
    }
}
