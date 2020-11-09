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
        public const string ERROR_CAPTCHA_UNSOLVABLE = "ERROR_CAPTCHA_UNSOLVABLE"; //TODO:Убрать отсюда
        /// <summary>
        /// Ошибка, плохие совпадения
        /// </summary>
        public const string ERROR_BAD_DUPLICATES = "ERROR_BAD_DUPLICATES"; //TODO:Убрать отсюда
        /// <summary>
        /// Ошибка, нулевой остаток (на расшифровку капчи)
        /// </summary>
        public const string ERROR_ZERO_BALANCE = "ERROR_ZERO_BALANCE"; //TODO:Убрать отсюда

        public void Initialization(string pathToProfile)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument($"--user-data-dir={pathToProfile}"); //Путь к папке с профилем
            options.AddArgument("--profile-directory=AutoBot"); //Профиль
            options.AddArgument("--start-maximized"); //Полностью открывает браузер
            options.AddArgument("--disable-notifications"); //Блокировка уведомлений
            options.AddAdditionalCapability("useAutomationExtension", false); //Скрывает расширение
            options.AddExcludedArgument("enable-automation"); //Скрывает панель "Браузером управляет автомат. ПО"

            _browser = new ChromeDriver("./BrowserSettings/netcoreapp2.0", options, TimeSpan.FromSeconds(200));
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
        /// Установить драйвер
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
            _browser.Navigate().GoToUrl(url); //The HTTP request to the remote WebDriver server for URL http://localhost:60218/session/157ce980ef266ec318973dab5b7a044d/url timed out after 200 seconds."
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
            Thread.Sleep(1500);
            var newTabInstance = _browser.WindowHandles[_browser.WindowHandles.Count - 1];
            _browser.SwitchTo().Window(newTabInstance);
        }
        /// <summary>
        /// Переключиться на вкладку
        /// </summary>
        /// <param name="indexTab">Индекс вкладки. По умолчанию нулевой</param>
        public void SwitchToTab(int indexTab = 0)
        {
            Thread.Sleep(1500);
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
        /// Переключиться на фрейм
        /// </summary>
        /// <param name="frame">IFrame-элемент</param>
        public void SwitchToFrame(IWebElement frame)
        {
            _browser.SwitchTo().Frame(frame);
        }
        /// <summary>
        /// Переключиться на основной контент страницы
        /// </summary>
        public void SwitchToDefaultContent()
        {
            _browser.SwitchTo().DefaultContent();
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


        /// <summary>
        /// Получить элемент по XPath
        /// </summary>
        /// <param name="xPath">Путь к элементу в DOM</param>
        /// <param name="waitingTimeSecond">Время ожидания</param>
        /// <returns>Вэб-элемент</returns>
        public IWebElement GetElementByXPath(string xPath, int waitingTimeSecond = 5)
        {
            while (waitingTimeSecond != 0)
            {
                var webElement = GetElementsByXPath(xPath);
                if (webElement.Any())
                {
                    return webElement.First();
                }

                Thread.Sleep(1000);
                waitingTimeSecond--;
            }

            return null;
        }
        /// <summary>
        /// Получить элемент по Id
        /// </summary>
        /// <param name="elementId">Наименование Id</param>
        /// <param name="waitingTimeSecond">Время ожидания</param>
        /// <returns>Вэб-элемент</returns>
        public IWebElement GetElementById(string elementId, int waitingTimeSecond = 5)
        {
            while (waitingTimeSecond != 0)
            {
                var webElement = _browser.FindElementById(elementId);
                if (webElement != null)
                {
                    return webElement;
                }

                Thread.Sleep(1000);
                waitingTimeSecond--;
            }

            return null;
        }
        /// <summary>
        /// Получить элемент по классу
        /// </summary>
        /// <param name="className">Наименование класса</param>
        /// <param name="waitingTimeSecond">Время ожидания</param>
        /// <returns>Вэб-элемент</returns>
        public IWebElement GetElementByClassName(string className, int waitingTimeSecond = 5)
        {
            while (waitingTimeSecond != 0)
            {
                var webElement = GetElementsByClassName(className);
                if (webElement.Any())
                {
                    return webElement.First();
                }

                Thread.Sleep(1000);
                waitingTimeSecond--;
            }

            return null;
        }        
        /// <summary>
        /// Получить элемент по тэгу
        /// </summary>
        /// <param name="tagName">Наименование тэга</param>
        /// <param name="waitingTimeSecond">Время ожидания</param>
        /// <returns>Вэб-элемент</returns>
        public IWebElement GetElementByTagName(string tagName, int waitingTimeSecond = 5)
        {
            while (waitingTimeSecond != 0)
            {
                var webElement = GetElementsByTagName(tagName);
                if (webElement.Any())
                {
                    return webElement.First();
                }

                Thread.Sleep(1000);
                waitingTimeSecond--;
            }

            return null;
        }


        /// <summary>
        /// Асинхронно получить элемент по Id
        /// </summary>
        /// <param name="elementId">Наименование Id</param>
        /// <param name="waitingTimeSecond">Время ожидания</param>
        /// <returns>Вэб-элемент</returns>
        public async Task<IWebElement> GetElementByIdAsync(string elementId, int waitingTimeSecond = 5)
        {
            return await Task.Run(() => GetElementById(elementId, waitingTimeSecond));
        }
        /// <summary>
        /// Асинхронно получить элемент по XPath
        /// </summary>
        /// <param name="xPath">Путь к элементу в DOM</param>
        /// <param name="waitingTimeSecond">Время ожидания</param>
        /// <returns>Вэб-элемент</returns>
        protected async Task<IWebElement> GetAsyncElementByXPath(string xPath, int waitingTimeSecond = 5)
        {
            return await Task.Run(() => GetElementByXPath(xPath, waitingTimeSecond));
        }


        /// <summary>
        /// Получить элементы по XPath
        /// </summary>
        /// <param name="xPath">Путь к коллекции в DOM</param>
        /// <returns>Коллекция вэб-элементов</returns>
        public IEnumerable<IWebElement> GetElementsByXPath(string xPath)
        {
            return _browser.FindElementsByXPath(xPath);
        }
        /// <summary>
        /// Получить элементы по классу
        /// </summary>
        /// <param name="className">Наименование класса</param>
        /// <returns>Коллекция вэб-элементов</returns>
        public IEnumerable<IWebElement> GetElementsByClassName(string className)
        {
            return _browser.FindElementsByClassName(className);
        }
        /// <summary>
        /// Получить элементы по тэгу
        /// </summary>
        /// <param name="tagName">Наименование тэга</param>
        /// <returns>Коллекция вэб-элементов</returns>
        public IEnumerable<IWebElement> GetElementsByTagName(string tagName)
        {
            return _browser.FindElementsByTagName(tagName);
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
        /// <returns>Титул страницы</returns>
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
            Thread.Sleep(3500);
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