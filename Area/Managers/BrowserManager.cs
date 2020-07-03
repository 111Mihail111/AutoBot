using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.Managers
{
    public class BrowserManager
    {
        private static readonly ChromeDriver Browser = Initialization(new ChromeOptions());
        
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
        /// Открыть страницу в новой вкладке
        /// </summary>
        /// <param name="url">Url-адрес страницы</param>
        public void OpenPageInNewTab(string url)
        {
            Browser.ExecuteScript("window.open();");
            SwitchToLastTab();
            Browser.Navigate().GoToUrl(url);
        }
        /// <summary>
        /// Закрыть вкладку
        /// </summary>
        public void CloseTab()
        {
            Browser.ExecuteScript("window.close();");
        }
        private void SwitchToLastTab()
        {
            var newTabInstance = Browser.WindowHandles[Browser.WindowHandles.Count - 1];
            Browser.SwitchTo().Window(newTabInstance);
        }
        public void SwitchToTab(int indexTab = 0)
        {
            Browser.SwitchTo().Window(Browser.WindowHandles[indexTab]);
        }

        /// <summary>
        /// Авторизация на сайте
        /// </summary>
        /// <param name="loginFieldId">Id поля логина</param>
        /// <param name="passwordFieldId">Id поля пароля</param>
        /// <param name="buttonId">Id кнопки</param>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void AuthorizationOnSite(string loginFieldId, string passwordFieldId, string buttonId, string login, string password)
        {
            GetElementById(loginFieldId).SendKeys(login);
            GetElementById(passwordFieldId).SendKeys(password);
            GetElementById(buttonId).Click();
        }
        /// <summary>
        /// Установить позицию скрола
        /// </summary>
        /// <param name="scrollTopPosition">Позиция верхнего скрола</param>
        public void SetScrollPosition(int scrollVerticalPosition  = 0, int scrollHorizontalPosition = 0)
        {
            Browser.ExecuteScript($"window.scroll({scrollHorizontalPosition}, {scrollVerticalPosition})");
        }
        /// <summary>
        /// Выполнить скрипт
        /// </summary>
        /// <param name="jsScript">Скрипт</param>
        public string ExecuteScript(string jsScript)
        {
            return Browser.ExecuteScript(jsScript)?.ToString() ?? string.Empty;
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


        /// <summary>
        /// Создать действие в браузере
        /// </summary>
        /// <returns>Действие</returns>
        public Actions CreateActionToBrowser()
        {
            return new Actions(Browser);
        }


        /// <summary>
        /// Получить URL страницы
        /// </summary>
        /// <returns>URL</returns>
        public string GetUrlPage()
        {
            return Browser.Url;
        }
        /// <summary>
        /// Получить титул страницы
        /// </summary>
        /// <returns>Титул</returns>
        public string GetTitlePage()
        {
            return Browser.Title;
        }



        #region Запуск хрома под другим профилем
        //РАБОЧИЙ ВАРИАНТ
        //option.AddArgument("--user-data-dir=C:\\Users\\PolovinkinMV\\AppData\\Local\\Google\\Chrome\\User Data\\");
        //option.AddArgument("--profile-directory=Profile 4");
        //option.AddArgument("--start-maximized");


        //ChromeOptions option = new ChromeOptions();

        //option.AddArgument("--user-data-dir=C:\\Users\\PolovinkinMV\\AppData\\Local\\Google\\Chrome\\User Data\\");
        //option.AddArgument("--load-extension=chrome-extension");
        //option.AddArgument("--profile-directory=Profile 4");
        //option.AddArgument("--disable-extensions");

        //option.AddArguments("--disable-infobars");
        //option.AddAdditionalCapability("useAutomationExtension", true);
        //option.AddArgument("--no-sandbox");
        //option.AddArgument("--headless");
        //option.AddArgument("--disable-dev-shm-usage");

        //option.AddArgument("--user-data-dir=C:\\Users\\PolovinkinMV\\AppData\\Local\\Google\\Chrome\\User Data\\");
        //    option.AddArgument("--profile-directory=Profile 4");
        //    option.AddArgument("--start-maximized");
        //    option.AddExtension("C:\\Project\\AutoBot\\WebExtention\\internal.crx");


        //    var Browser = new ChromeDriver("/Project/AutoBot/bin/Debug/netcoreapp2.0", option, TimeSpan.FromSeconds(200));
        #endregion Запуск хрома под другим профилем
    }
}