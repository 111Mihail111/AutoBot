using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class InstagramManager : BrowserManager, IInstagramManager
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            OpenPageInNewTab("https://www.instagram.com/");

            var loginForm = GetElementById("loginForm");
            if (loginForm == null)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            var inputs = loginForm.FindElements(SearchMethod.Tag, "input");
            foreach (var item in inputs)
            {
                if (item.GetName() == "username")
                {
                    if (string.IsNullOrWhiteSpace(item.GetValue()))
                    {
                        item.SendKeys(login);
                    }
                }
                else if (item.GetName() == "password")
                {
                    if (string.IsNullOrWhiteSpace(item.GetValue()))
                    {
                        item.SendKeys(password);
                    }
                }
            }

            var buttons = loginForm.FindElements(SearchMethod.Tag, "button");
            foreach (var item in buttons)
            {
                if (item.GetInnerText() == "Войти")
                {
                    item.Click();
                    Thread.Sleep(3000);
                    break;
                }
            }

            CloseTab();
            SwitchToLastTab();
        }
        /// <inheritdoc/>
        public void Subscribe()
        {
            var buttons = GetElementsByTagName("button");
            foreach (var item in buttons)
            {
                if (item.GetInnerText() == "Подписаться")
                {
                    item.Click();
                    Thread.Sleep(2000);
                    return;
                }
            }
        }
        /// <inheritdoc/>
        public void Unsubscribe()
        {
            string canselSubscribeButtonClickScript = "document.getElementsByClassName('aOOlW -Cab_   ')[0].click();";

            var buttons = GetElementsByTagName("button");
            foreach (var item in buttons)
            {
                var spanCollection = item.FindElements(SearchMethod.Tag, "span");
                if (item.GetInnerText() == "Запрос отправлен")
                {
                    item.Click();
                    Thread.Sleep(400);

                    ExecuteScript(canselSubscribeButtonClickScript);
                    break;
                }
                else if (spanCollection.Any())
                {
                    if (spanCollection.First().GetAriaLabel() == "Подписки")
                    {
                        item.Click();
                        Thread.Sleep(400);

                        ExecuteScript(canselSubscribeButtonClickScript);
                        break;
                    }
                }
            }

            Thread.Sleep(2000);
        }
        /// <inheritdoc/>
        public void PutLike()
        {
            var svgCollection = GetElementsByClassName("_8-yf5");
            foreach (var item in svgCollection)
            {
                if (item.GetAriaLabel() == "Нравится")
                {
                    item.Click();
                    Thread.Sleep(2000);
                    return;
                }
            }
        }
        /// <inheritdoc/>
        public void RemoveLike()
        {
            var svgCollection = GetElementsByClassName("_8-yf5");
            foreach (var item in svgCollection)
            {
                if (item.GetAriaLabel() == "Не нравится")
                {
                    item.Click();
                    Thread.Sleep(2000);
                    return;
                }
            }
        }
        /// <inheritdoc/>
        public bool IsFoundPage()
        {
            return !GetTitlePage().Contains("Страница не найдена") != false;
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
