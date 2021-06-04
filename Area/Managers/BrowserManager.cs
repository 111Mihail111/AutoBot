using AutoBot.Area.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.Managers
{
    public class BrowserManager
    {
        private ChromeDriver _chromeDriver;
        private bool _isHeadless;

        public void Initialization(string pathToProfile)
        {
            var options = new ChromeOptions();
            options.AddArgument($"--user-data-dir={pathToProfile}"); //Путь к папке с профилем
            options.AddArgument("--profile-directory=AutoBot"); //Профиль
            options.AddArgument("--start-maximized"); //Разворачивает браузер на ширину экрана
            options.AddArgument("--disable-notifications"); //Блокировка уведомлений
            options.AddArgument("--mute-audio"); //Отключает звук в браузере

            options.BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"; //Для дом. запуска
            options.AddAdditionalCapability("useAutomationExtension", false); //Скрывает указанное расширение
            options.AddExcludedArgument("enable-automation"); //Скрывает панель "Браузером управляет автомат. ПО"

            _chromeDriver = new ChromeDriver("./BrowserSettings/netcoreapp2.0", options, TimeSpan.FromSeconds(300));
        }

        public void Initialization(string pathToProfile, bool isHeadless)
        {
            var options = new ChromeOptions();
            options.AddArgument($"--user-data-dir={pathToProfile}"); //Путь к папке с профилем
            options.AddArgument("--profile-directory=AutoBot"); //Профиль
            options.AddArgument("--window-size=1920,1080"); //Разворачивает браузер на указаную ширину экрана
            options.AddArgument("--disable-notifications"); //Блокировка уведомлений
            options.AddArgument("--mute-audio"); //Отключает звук в браузере
            options.AddArgument("--headless"); //Запуск в фоновом режиме (без отображения бразуера)
            options.AddArgument("--no-sandbox"); //Отключает безопасность хрома, для корректной работы js-скриптов

            options.BinaryLocation = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"; //Для дом. запуска
            options.AddAdditionalCapability("useAutomationExtension", false); //Скрывает указанное расширение
            options.AddExcludedArgument("enable-automation"); //Скрывает панель "Браузером управляет автомат. ПО"

            _chromeDriver = new ChromeDriver("./BrowserSettings/netcoreapp2.0", options, TimeSpan.FromSeconds(300));
            _isHeadless = isHeadless;
        }

        /// <summary>
        /// Получить объект драйвера
        /// </summary>
        /// <returns>Хром-драйвер</returns>
        public ChromeDriver GetDriver()
        {
            return _chromeDriver;
        }
        /// <summary>
        /// Установить драйвер
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetDriver(ChromeDriver chromeDriver)
        {
            _chromeDriver = chromeDriver;
        }


        /// <summary>
        /// Перейти по адресу
        /// </summary>
        /// <param name="url">Адрес</param>
        public void GoToUrl(string url)
        {
            _chromeDriver.Navigate().GoToUrl(url);
        }
        /// <summary>
        /// Открыть страницу в новой вкладке
        /// </summary>
        /// <param name="url">Url-адрес страницы</param>
        public void OpenPageInNewTab(string url)
        {
            Thread.Sleep(2000);
            _chromeDriver.ExecuteScript($"window.open('{url}');");
            SwitchToLastTab();
        }
        /// <summary>
        /// Закрыть вкладку
        /// </summary>
        public void CloseTab()
        {
            _chromeDriver.Close();
        }
        /// <summary>
        /// Переключиться на последнюю вкладку
        /// </summary>
        public void SwitchToLastTab()
        {
            Thread.Sleep(1500);
            var newTabInstance = _chromeDriver.WindowHandles[_chromeDriver.WindowHandles.Count - 1];
            _chromeDriver.SwitchTo().Window(newTabInstance);
        }
        /// <summary>
        /// Переключиться на вкладку
        /// </summary>
        /// <param name="indexTab">Индекс вкладки. По умолчанию нулевой</param>
        public void SwitchToTab(int indexTab = 0)
        {
            Thread.Sleep(1500);
            _chromeDriver.SwitchTo().Window(_chromeDriver.WindowHandles[indexTab]);
        }
        /// <summary>
        /// Закрыть текущую вкладку и переключиться на другую
        /// </summary>
        /// <param name="indexTab">Индекс вкладки</param>
        public void CloseCurrentTabAndSwitchToAnother(int indexTab = 0)
        {
            CloseTab();
            SwitchToTab(indexTab);
        }
        /// <summary>
        /// Получить количество вкладок
        /// </summary>
        /// <returns>Количество вкладок</returns>
        public int GetTabsCount()
        {
            return _chromeDriver.WindowHandles.Count;
        }
        /// <summary>
        /// Переключиться на фрейм
        /// </summary>
        /// <param name="frame">IFrame-элемент</param>
        public void SwitchToFrame(IWebElement frame)
        {
            _chromeDriver.SwitchTo().Frame(frame);
        }
        /// <summary>
        /// Переключиться на основной контент страницы
        /// </summary>
        public void SwitchToDefaultContent()
        {
            _chromeDriver.SwitchTo().DefaultContent();
        }

        /// <summary>
        /// ОК - в алерт окне
        /// </summary>
        public void AlertAccept()
        {
            _chromeDriver.SwitchTo().Alert().Accept();
        }
        /// <summary>
        /// Открыт ли Alert на странице
        /// </summary>
        /// <returns>True - открыт, иначе false</returns>
        public bool IsAlertExist(int waitingTimeSecond = 2)
        {
            while (waitingTimeSecond != 0)
            {
                try
                {
                    _chromeDriver.SwitchTo().Alert();
                    return true;
                }
                catch (NoAlertPresentException)
                {
                    Thread.Sleep(1000);
                    waitingTimeSecond--;
                }
            }

            return false;
        }
        /// <summary>
        /// Получить текст из алерт окна
        /// </summary>
        /// <returns></returns>
        public string GetTextFromAlert()
        {
            try
            {
                return _chromeDriver.SwitchTo().Alert().Text;
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
            _chromeDriver.ExecuteScript($"window.scroll({scrollHorizontalPosition}, {scrollVerticalPosition})");
        }
        /// <summary>
        /// Установить позицию скрола в модальном окне
        /// </summary>
        /// <param name="modalId">Идентификатор окна</param>
        /// <param name="scrollVerticalPosition">Позиция верхнего скрола</param>
        /// <param name="scrollHorizontalPosition">Позиция нижнего скрола</param>
        public void SetScrollPositionInWindow(string modalId, int scrollVerticalPosition = 0, int scrollHorizontalPosition = 0)
        {
            _chromeDriver.ExecuteScript($"let modal = document.getElementById('{modalId}');" +
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
            return _chromeDriver.ExecuteScript(jsScript)?.ToString();
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
                var webElement = GetElementsById(elementId);
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
        /// Получить элемент по CssSelecor'y
        /// </summary>
        /// <returns>Вэб-элемент</returns>
        public IWebElement GetElementByCssSelector(string cssSelector, int waitingTimeSecond = 5)
        {
            while (waitingTimeSecond != 0)
            {
                var webElement = GetElementsByCssSelector(cssSelector);
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
        /// Получить элементы по Id
        /// </summary>
        /// <param name="elementId">Наименование Id</param>
        /// <returns>Коллекция вэб-элементов</returns>
        public IEnumerable<IWebElement> GetElementsById(string elementId)
        {
            return _chromeDriver.FindElementsById(elementId);
        }
        /// <summary>
        /// Получить элементы по XPath
        /// </summary>
        /// <param name="xPath">Путь к коллекции в DOM</param>
        /// <returns>Коллекция вэб-элементов</returns>
        public IEnumerable<IWebElement> GetElementsByXPath(string xPath)
        {
            return _chromeDriver.FindElementsByXPath(xPath);
        }
        /// <summary>
        /// Получить элементы по классу
        /// </summary>
        /// <param name="className">Наименование класса</param>
        /// <returns>Коллекция вэб-элементов</returns>
        public IEnumerable<IWebElement> GetElementsByClassName(string className)
        {
            return _chromeDriver.FindElementsByClassName(className);
        }
        /// <summary>
        /// Получить элементы по тэгу
        /// </summary>
        /// <param name="tagName">Наименование тэга</param>
        /// <returns>Коллекция вэб-элементов</returns>
        public IEnumerable<IWebElement> GetElementsByTagName(string tagName)
        {
            return _chromeDriver.FindElementsByTagName(tagName);
        }
        /// <summary>
        /// Получить элементу по селектору
        /// </summary>
        /// <param name="cssSelector">Наименование селектора</param>
        /// <returns>Коллекция вэб-элементов</returns>
        public IEnumerable<IWebElement> GetElementsByCssSelector(string cssSelector)
        {
            return _chromeDriver.FindElementsByCssSelector(cssSelector);
        }


        /// <summary>
        /// Получить URL страницы
        /// </summary>
        /// <returns>URL</returns>
        public string GetUrlPage()
        {
            return _chromeDriver.Url;
        }
        /// <summary>
        /// Получить титул страницы
        /// </summary>
        /// <returns>Титул страницы</returns>
        public string GetTitlePage()
        {
            return _chromeDriver.Title;
        }
        /// <summary>
        /// Обновить страницу
        /// </summary>
        public void RefreshPage()
        {
            _chromeDriver.Navigate().Refresh();
            Thread.Sleep(3500);
        }
        /// <summary>
        /// Выйти из браузера
        /// </summary>
        public void QuitBrowser()
        {
            _chromeDriver.Close();
            _chromeDriver.Quit();
        }


        /// <summary>
        /// Перейти к элементу и нажать
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        public void MoveToElementAndClick(IWebElement webElement)
        {
            Actions action = new Actions(_chromeDriver);
            action.MoveToElement(webElement).Click().Build().Perform();
        }
        /// <summary>
        /// Фокус на элемент
        /// </summary>
        /// <param name="webElement">Веб-элемент</param>
        public void FocusOnElement(IWebElement webElement)
        {
            Actions action = new Actions(_chromeDriver);
            action.MoveToElement(webElement).Build().Perform();
        }


        /// <summary>
        /// Получить скриншот
        /// </summary>
        /// <returns>Изображение экрана</returns>
        public Screenshot GetScreenshot()
        {
            return _chromeDriver.GetScreenshot();
        }
    }
}