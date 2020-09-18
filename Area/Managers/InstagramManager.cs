using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class InstagramManager : BrowserManager, IInstagramManager
    {
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

        public void Unsubscribe()
        {
            var buttons = GetElementsByTagName("button");
            foreach (var item in buttons)
            {
                var span = item.FindElement(SearchMethod.Tag, "span");
                if (span.GetAriaLabel() == "Подписки")
                {
                    item.Click();

                    GetElementByClassName("aOOlW -Cab_   ").Click();
                    Thread.Sleep(2000);
                }
            }
        }

        public void PutLike()
        {
            var buttons = GetElementsByClassName("wpO6b");
            foreach (var item in buttons)
            {
                var svg = item.FindElement(SearchMethod.Tag, "svg");
                if (svg.GetAriaLabel() == "Нравится")
                {
                    svg.Click();
                    Thread.Sleep(2000);
                    return;
                }
            }
        }

        public bool IsFoundPage()
        {
            return !GetTitlePage().Contains("Страница не найдена") != false;
        }

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            GetDriver(chromeDriver);
        }
    }
}
