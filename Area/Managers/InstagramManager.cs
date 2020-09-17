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

        }

        public bool IsFoundPage()
        {
            return GetTitlePage().Contains("Страница не найдена") == false;
        }

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            GetDriver(chromeDriver);
        }
    }
}
