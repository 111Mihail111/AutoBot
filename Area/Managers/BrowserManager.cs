using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AutoBot.Area.Managers
{
    public class BrowserManager
    {
        public static readonly ChromeDriver Browser = Initialization();

        public static ChromeDriver Initialization()
        {
            ChromeOptions options = new ChromeOptions();
            options.SetLoggingPreference(LogType.Browser, LogLevel.All);
            return new ChromeDriver("/Project/AutoBot/bin/Debug/netcoreapp2.0", options, TimeSpan.FromSeconds(200));
        }

        /// <summary>
        /// Перейти по адресу
        /// </summary>
        /// <param name="url">Адрес</param>
        public void GoToUrl(string url)
        {
            Browser.Navigate().GoToUrl(url);
        }
        /// <summary>
        /// Авторизация на кране
        /// </summary>
        /// <param name="loginFieldId">Id поля логина</param>
        /// <param name="passwordFieldId">Id поля пароля</param>
        /// <param name="buttonId">Id кнопки</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void AuthorizationOnCrane(string loginFieldId, string passwordFieldId, string buttonId, string login, string password)
        {
            Browser.FindElementById(loginFieldId).SendKeys(login);
            Browser.FindElementById(passwordFieldId).SendKeys(password);
            Browser.FindElementById(buttonId).Click();
        }
        /// <summary>
        /// Установить позицию верхнего скрола
        /// </summary>
        /// <param name="scrollTopPosition">Позиция верхнего скрола</param>
        public void SetPositionScrollTop(int scrollTopPosition)
        {
            Browser.ExecuteScript($"window.scroll({"{"}{scrollTopPosition}{"}"})");
        }

        public IWebElement GetElementByXPath(string xPath)
        {
            return Browser.FindElementByXPath(xPath);
        }

        public IEnumerable<IWebElement> GetElementsByXPath(string xPath)
        {
            return Browser.FindElementsByXPath(xPath);
        }
    }
}
