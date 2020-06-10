using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class BrowserManager
    {
        public static readonly ChromeDriver Browser = Initialization(new ChromeOptions());

        public static ChromeDriver Initialization(ChromeOptions options)
        {
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
        public void SetScrollPosition(int scrollVerticalPosition = 0, int scrollHorizontalPosition = 0)
        {
            Browser.ExecuteScript($"window.scroll({scrollHorizontalPosition}, {scrollVerticalPosition})");
        }


        public IWebElement GetElementByXPath(string xPath, int waitingTimeSecond = 5)
        {
            return ExpectationElement(xPath, waitingTimeSecond);
        }
        public IEnumerable<IWebElement> GetElementsByXPath(string xPath)
        {
            return Browser.FindElementsByXPath(xPath);
        }


        public IWebElement GetElementById(string elementId, int waitingTimeSecond = 5)
        {
            return ExpectationElement(elementId, waitingTimeSecond);
        }
        public IEnumerable<IWebElement> GetElementsById(string elementId)
        {
            return Browser.FindElementsById(elementId);
        }


        /// <summary>
        /// Ожидание элемента на странице
        /// </summary>
        /// <param name="attributeElement">Атрибут, по которому искать элемент</param>
        /// <param name="waitingTimeSecond">Время ожидания элемента</param>
        /// <returns>Веб-элемент</returns>
        protected IWebElement ExpectationElement(string attributeElement, int waitingTimeSecond)
        {
            while (waitingTimeSecond != 0)
            {
                var webElementId = GetElementsById(attributeElement);
                if (webElementId.Any())
                {
                    return webElementId.First();
                }

                var webElementXPath = GetElementsByXPath(attributeElement);
                if (webElementXPath.Any())
                {
                    return webElementXPath.First();
                }

                Thread.Sleep(1000);
                waitingTimeSecond--;
            }

            //TODO: ОБЕРНУТЬ В TRY CATCH И ОЖИДАТЬ ОШИБКУ ЗДЕСЬ, если элемент не найден
            return null;
        }
    }
}