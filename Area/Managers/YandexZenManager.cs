using AutoBot.Area.Managers.Interface;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class YandexZenManager : BrowserManager, IYandexZenManager
    {
        public void Authorization(string login, string password)
        {
            string url = "https://zen.yandex.ru/user/";
            OpenPageInNewTab(url);

            if (GetUrlPage().Contains(url))
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            var conservedAccount = GetElementByClassName("AuthAccountListItem");
            if (conservedAccount != null)
            {
                conservedAccount.Click();
                AuthorizationUnderConservedAccount(password);
                return;
            }
            else if (GetElementByClassName("CurrentAccount") != null)
            {
                AuthorizationUnderConservedAccount(password);
                return;
            }

            var loginInput = GetElementById("passp-field-login");
            if (string.IsNullOrWhiteSpace(loginInput.Text))
            {
                loginInput.SendKeys(login);
            }

            GetElementByClassName("Button2_view_action").Click();
            Thread.Sleep(1500);

            var passwordInput = GetElementById("passp-field-passwd");
            if (string.IsNullOrWhiteSpace(passwordInput.Text))
            {
                passwordInput.SendKeys(password);
            }

            GetElementByClassName("Button2_view_action").Click();
            Thread.Sleep(2000);

            CloseTab();
            SwitchToTab();
        }

        public void PutLike()
        {
            GetElementByXPath("//*[@id='article__left']/div/div/div/div[1]/div[1]/div[1]/button").Click();
            Thread.Sleep(2000);
        }

        public void RemoveLike()
        {
            GetElementByXPath("//*[@id='article__left']/div/div/div/div[1]/div[1]/div[1]/button").Click();
            Thread.Sleep(2000);
        }

        public void Subscribe()
        {
            GetElementByClassName("_view-type_rounded-yellow").Click();
            Thread.Sleep(2000);
        }

        public void Unsubscribe()
        {
            GetElementByClassName("_view-type_rounded-white").Click();
            Thread.Sleep(2000);
        }

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }

        /// <summary>
        /// Авторизация под сохраненным аккаунтом
        /// </summary>
        /// <param name="password">Пароль к аккаунту</param>
        protected void AuthorizationUnderConservedAccount(string password)
        {
            var passwordInputInSaveAccount = GetElementById("passp-field-passwd");
            if (string.IsNullOrWhiteSpace(passwordInputInSaveAccount.Text))
            {
                passwordInputInSaveAccount.SendKeys(password);
            }

            GetElementByClassName("Button2_view_action").Click();
            Thread.Sleep(2000);

            CloseTab();
            SwitchToTab();
        }
    }
}
