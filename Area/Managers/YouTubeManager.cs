using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class YouTubeManager : BrowserManager, IYouTubeManager
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            string url = "https://www.youtube.com/account";
            OpenPageInNewTab(url);

            if (GetUrlPage() == url)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            var savedAccountDiv = GetElementById("profileIdentifier");
            if (savedAccountDiv != null)
            {
                savedAccountDiv.ToClick();
                AuthorizationUnderSavedProfile(password);
                return;
            }

            var inputLogin = GetElementById("identifierId");
            if (string.IsNullOrWhiteSpace(inputLogin.GetValue()))
            {
                inputLogin.SendKeys(login);
            }
            GetElementById("identifierNext").FindElement(SearchMethod.Tag, "button").ToClick(2000);

            var inputPassword = GetElementById("password").FindElement(SearchMethod.Tag, "input");
            if (string.IsNullOrWhiteSpace(inputPassword.GetValue()))
            {
                inputPassword.SendKeys(password);
            }
            GetElementById("passwordNext").FindElement(SearchMethod.Tag, "button").ToClick(2000);

            GoToUrl(url);
            ButtonLoginClick();

            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void AuthorizationForOldVersionBrowser(string login, string password)
        {
            string url = "https://www.youtube.com/account";
            OpenPageInNewTab(url);

            if (GetUrlPage() == url)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            RemoveAccountDetailsOldVersionBrowser(login);

            var inputLogin = GetElementById("Email");
            if (string.IsNullOrWhiteSpace(inputLogin.GetValue()))
            {
                inputLogin.SendKeys(login);
            }
            GetElementById("next").ToClick(2000);

            var inputPassword = GetElementById("password");
            if (string.IsNullOrWhiteSpace(inputPassword.GetValue()))
            {
                inputPassword.SendKeys(password);
            }
            GetElementById("submit").ToClick(2000);

            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void SubscribeToChannel()
        {
            GetElementByXPath("//*[@id='subscribe-button']/ytd-subscribe-button-renderer/paper-button").ToClick(1500);
        }
        /// <inheritdoc/>
        public void UnsubscribeFromChannel()
        {
            GetElementByXPath("//*[@id='subscribe-button']/ytd-subscribe-button-renderer/paper-button").ToClick(1500);

            var modalWindow = GetElementByXPath("//paper-dialog/yt-confirm-dialog-renderer");
            modalWindow.FindElements(SearchMethod.Tag, "a").Last().ToClick(2000);
        }
        /// <inheritdoc/>
        public void LikeUnderVideo()
        {
            StopVideoAsync();
            RemoveModalDialogs();

            GetElementByXPath("//*[@id='top-level-buttons']/ytd-toggle-button-renderer[1]/a").Click();
            Thread.Sleep(2000);
        }
        /// <inheritdoc/>
        public void RemoveLike()
        {
            StopVideoAsync();
            RemoveModalDialogs();

            GetElementByXPath("//*[@id='top-level-buttons']/ytd-toggle-button-renderer[1]/a").Click();
            Thread.Sleep(2000);
        }
        /// <inheritdoc/>
        public void DislikeUnderVideo()
        {
            StopVideoAsync();
            GetElementByXPath("//*[@id='top-level-buttons']/ytd-toggle-button-renderer[2]/a").Click();
            Thread.Sleep(2000);
        }
        /// <inheritdoc/>
        public void RemoveDislike()
        {
            StopVideoAsync();
            GetElementByXPath("//*[@id='top-level-buttons']/ytd-toggle-button-renderer[2]/a").Click();
            Thread.Sleep(1500);
        }
        /// <inheritdoc/>
        public bool IsVideoAvailable()
        {
            return GetElementById("reason") == null;
        }
        /// <inheritdoc/>
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
        /// Удалить детали аккаунта в старой версии браузера
        /// </summary>
        /// <param name="login">Логин</param>
        protected void RemoveAccountDetailsOldVersionBrowser(string login)
        {
            var savedAccountLi = GetElementById($"account-{login}");
            if (savedAccountLi != null)
            {
                GetElementById("edit-account-list").ToClick(1000);
                GetElementById("choose-account-0").ToClick(1000);
                GetElementById("edit-account-list").ToClick(1000);
            }
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
            ExecuteScript("document.getElementsByTagName('paper-dialog')[0]?.remove();");
        }
        /// <summary>
        /// Остановить видео
        /// </summary>
        protected async void StopVideoAsync()
        {
            await ExecuteScriptAsync("document.getElementsByClassName('ytp-play-button')[0].click();");
        }
    }
}
