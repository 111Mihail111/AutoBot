using AutoBot.Area.Enums;
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
        private ChromeDriver _browser;

        /// <summary>
        /// Ошибка, капча неразрешима
        /// </summary>
        public const string ERROR_CAPTCHA_UNSOLVABLE = "ERROR_CAPTCHA_UNSOLVABLE";
        /// <summary>
        /// Ошибка, плохие совпадения
        /// </summary>
        public const string ERROR_BAD_DUPLICATES = "ERROR_BAD_DUPLICATES";
        /// <summary>
        /// Ошибка, нулевой остаток (на расшифровку капчи)
        /// </summary>
        public const string ERROR_ZERO_BALANCE = "ERROR_ZERO_BALANCE";

        #region Path for driver
        //"/Project/AutoBot/bin/Debug/netcoreapp2.0" - на работе
        //"/_VS_Project/Mihail/AutoBot/BrowserSettings/netcoreapp2.0" - дома
        #endregion Path for driver


        public void Initialization(string pathToProfile)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument($"--user-data-dir={pathToProfile}"); //Путь к папке с профилем
            options.AddArgument("--profile-directory=AutoBot"); //Профиль
            options.AddArgument("--start-maximized"); //Полностью открывает браузер
            options.AddAdditionalCapability("useAutomationExtension", false); //Скрывает расширение
            options.AddExcludedArgument("enable-automation"); //Скрывает панель "Браузером управляет автомат. ПО"

            _browser = new ChromeDriver("/Project/AutoBot/bin/Debug/netcoreapp2.0", options, TimeSpan.FromSeconds(200));
        }

        /// <summary>
        /// Получить объект драйвера
        /// </summary>
        /// <returns>Хром-драйвер</returns>
        public ChromeDriver GetDriver()
        {
            return _browser;
        }
        /// <summary>
        /// Считать драйвер
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetDriver(ChromeDriver chromeDriver)
        {
            _browser = chromeDriver;
        }


        /// <summary>
        /// Перейти по адресу
        /// </summary>
        /// <param name="url">Адрес</param>
        public void GoToUrl(string url)
        {
            _browser.Navigate().GoToUrl(url);
        }
        /// <summary>
        /// Открыть страницу в новой вкладке
        /// </summary>
        /// <param name="url">Url-адрес страницы</param>
        public void OpenPageInNewTab(string url)
        {
            Thread.Sleep(2000);
            _browser.ExecuteScript($"window.open('{url}');");
            SwitchToLastTab();
        }
        /// <summary>
        /// Закрыть вкладку
        /// </summary>
        public void CloseTab()
        {
            _browser.Close();
        }
        /// <summary>
        /// Переключиться на последнюю вкладку
        /// </summary>
        public void SwitchToLastTab()
        {
            var newTabInstance = _browser.WindowHandles[_browser.WindowHandles.Count - 1];
            _browser.SwitchTo().Window(newTabInstance);
        }
        /// <summary>
        /// Переключиться на вкладку
        /// </summary>
        /// <param name="indexTab">Индекс вкладки. По умолчанию нулевой</param>
        public void SwitchToTab(int indexTab = 0)
        {
            _browser.SwitchTo().Window(_browser.WindowHandles[indexTab]);
        }
        /// <summary>
        /// Получить количество вкладок
        /// </summary>
        /// <returns>Количество вкладок</returns>
        public int GetTabsCount()
        {
            return _browser.WindowHandles.Count;
        }

        /// <summary>
        /// ОК - в алерт окне
        /// </summary>
        public void AlertAccept()
        {
            _browser.SwitchTo().Alert().Accept();
        }
        /// <summary>
        /// Получить текст из алерт окна
        /// </summary>
        /// <returns></returns>
        public string GetTextFromAlert()
        {
            try
            {
                return _browser.SwitchTo().Alert().Text;
            }
            catch
            {
                return string.Empty;
            }

        }


        /// <summary>
        /// Установить позицию скрола
        /// </summary>
        /// <param name="scrollVerticalPosition">Позиция верхнего скрола</param>
        /// <param name="scrollHorizontalPosition">Позиция нижнего скрола</param>
        public void SetScrollPosition(int scrollVerticalPosition = 0, int scrollHorizontalPosition = 0)
        {
            _browser.ExecuteScript($"window.scroll({scrollHorizontalPosition}, {scrollVerticalPosition})");
        }
        /// <summary>
        /// Установить позицию скрола в модальном окне
        /// </summary>
        /// <param name="modalId">Идентификатор окна</param>
        /// <param name="scrollVerticalPosition">Позиция верхнего скрола</param>
        /// <param name="scrollHorizontalPosition">Позиция нижнего скрола</param>
        public void SetScrollPositionInWindow(string modalId, int scrollVerticalPosition = 0, int scrollHorizontalPosition = 0)
        {
            _browser.ExecuteScript($"let modal = document.getElementById('{modalId}');" +
                $"modal.scrollTop = {scrollVerticalPosition};" +
                $"modal.scrollHeight = {scrollHorizontalPosition}");
        }
        /// <summary>
        /// Выполнить скрипт
        /// </summary>
        /// <param name="jsScript">Скрипт</param>
        /// <returns>Возвращаемое значение скрипта</returns>
        public string ExecuteScript(string jsScript)
        {
            return _browser.ExecuteScript(jsScript)?.ToString();
        }
        /// <summary>
        /// Асинхронно выполнить скрипт
        /// </summary>
        /// <param name="jsScript">Скрипт</param>
        /// <returns>Возвращаемое значение js кода</returns>
        public async Task<string> ExecuteScriptAsync(string jsScript)
        {
            return await Task.Run(() => ExecuteScript(jsScript)?.ToString());
        }

        public IWebElement GetElementByXPath(string xPath, int waitingTimeSecond = 5)
        {
            return ExpectationElement(xPath, waitingTimeSecond);
        }
        public IEnumerable<IWebElement> GetElementsByXPath(string xPath)
        {
            //TODO:Найти код ошибки и воткнуть в кэч. Он не может адекватно искать из-за классов с нижним подчеркиванием
            try
            {
                return _browser.FindElementsByXPath(xPath);
            }
            catch
            {
                return new List<IWebElement>();
            }
        }
        protected async Task<IWebElement> GetAsyncElementByXPath(string xPath, int waitingTimeSecond = 5)
        {
            return await Task.Run(() => ExpectationElement(xPath, waitingTimeSecond));
        }


        public IWebElement GetElementById(string elementId, int waitingTimeSecond = 5)
        {
            return ExpectationElement(elementId, waitingTimeSecond);
        }
        public IEnumerable<IWebElement> GetElementsById(string elementId)
        {
            return _browser.FindElementsById(elementId);
        }
        public async Task<IWebElement> GetAsyncElementById(string elementId, int waitingTimeSecond = 5)
        {
            return await Task.Run(() => ExpectationElement(elementId, waitingTimeSecond));
        }


        public IWebElement GetElementByClassName(string className, int waitingTimeSecond = 5)
        {
            return ExpectationElement(className, waitingTimeSecond);
        }
        public IEnumerable<IWebElement> GetElementsByClassName(string className)
        {
            //TODO:Найти код ошибки и воткнуть в кэч. Он не может адекватно искать из-за символом xPath
            try
            {
                return _browser.FindElementsByClassName(className);
            }
            catch
            {
                return new List<IWebElement>();
            }

        }

        public IWebElement GetElementByTagName(string tagName, int waitingTimeSecond = 5)
        {
            return ExpectationElement(tagName, waitingTimeSecond);
        }
        public IEnumerable<IWebElement> GetElementsByTagName(string tagName)
        {
            return _browser.FindElementsByTagName(tagName);
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

                var webElementClassName = GetElementsByClassName(attributeElement);
                if (webElementClassName.Any())
                {
                    return webElementClassName.First();
                }

                var webElementTagName = GetElementsByTagName(attributeElement);
                if (webElementTagName.Any())
                {
                    return webElementTagName.First();
                }

                Thread.Sleep(1000);
                waitingTimeSecond--;
            }

            return null;
        }


        /// <summary>
        /// Получить URL страницы
        /// </summary>
        /// <returns>URL</returns>
        public string GetUrlPage()
        {
            return _browser.Url;
        }
        /// <summary>
        /// Получить титул страницы
        /// </summary>
        /// <returns>Титул</returns>
        public string GetTitlePage()
        {
            return _browser.Title;
        }
        /// <summary>
        /// Обновить страницу
        /// </summary>
        public void RefreshPage()
        {
            _browser.Navigate().Refresh();
        }
        /// <summary>
        /// Выйти из браузера
        /// </summary>
        public void QuitBrowser()
        {
            _browser.Quit();
        }


        /// <summary>
        /// Перейти к элементу и нажать
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        public void MoveToElementAndClick(IWebElement webElement)
        {
            Actions action = new Actions(_browser);
            action.MoveToElement(webElement).Click().Build().Perform();
        }
        /// <summary>
        /// Фокус на элемент
        /// </summary>
        /// <param name="webElement">Веб-элемент</param>
        public void FocusOnElement(IWebElement webElement)
        {
            Actions action = new Actions(_browser);
            action.MoveToElement(webElement).Build().Perform();
        }
    }
}