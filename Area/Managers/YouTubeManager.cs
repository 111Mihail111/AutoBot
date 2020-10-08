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

        public void SubscribeToChannel() //TODO:1 Дублируется часть кода. Совместить в один метод
        {
            GetElementByXPath("//*[@id='subscribe-button']/ytd-subscribe-button-renderer/paper-button").Click();
            Thread.Sleep(1500);
        }

        public void UnsubscribeFromChannel() //TODO:1 Дублируется часть кода. Совместить в один метод
        {
            GetElementByXPath("//*[@id='subscribe-button']/ytd-subscribe-button-renderer/paper-button").Click();
            Thread.Sleep(1500);

            GetElementById("main").FindElements(SearchMethod.Tag, "paper-button").Last().Click();
            Thread.Sleep(2000);
        }

        public void LikeUnderVideo() //TODO:2 Дублируется код. Совместить в один метод
        {
            RemoveModalDialogs();
            GetElementByXPath("//*[@id='top-level-buttons']/ytd-toggle-button-renderer[1]/a").Click();
            Thread.Sleep(2000);
        }

        public void RemoveLike() //TODO:2 Дублируется код. Совместить в один метод
        {
            RemoveModalDialogs();
            GetElementByXPath("//*[@id='top-level-buttons']/ytd-toggle-button-renderer[1]/a").Click();
            Thread.Sleep(2000);
        }

        public void DislikeUnderVideo()
        {
            GetElementByXPath("//*[@id='top-level-buttons']/ytd-toggle-button-renderer[2]/a").Click();
            Thread.Sleep(2000);
        }

        public void RemoveDislike()
        {
            GetElementByXPath("//*[@id='top-level-buttons']/ytd-toggle-button-renderer[2]/a").Click();
            Thread.Sleep(1500);
        }

        public bool IsVideoAvailable()
        {
            return GetElementById("reason") == null;
        }

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
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
        /// <summary>
        /// Удалить диалоговые окна
        /// </summary>
        protected void RemoveModalDialogs()
        {
            ExecuteScript("var modalDialog = document.getElementsByTagName('paper-dialog')[0];" +
                "if (modalDialog != undefined)" +
                "{" +
                    "modalDialog.remove();" +
                "}");
        }
    }
}
