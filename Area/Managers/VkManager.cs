using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class VkManager : BrowserManager, IVkManager
    {
        public void JoinToComunity()
        {
            var joinButton = GetElementById("join_button");
            if (joinButton == null)
            {
                GetElementById("public_subscribe").Click();
                Thread.Sleep(1500);
                return;
            }
            joinButton.Click();
            Thread.Sleep(1500);
        }

        public void PutLike()
        {
            var post = GetUrlPage().Replace("https://vk.com/wall", "post");
            var buttons = GetElementsByXPath($"//*[@id='{post}']/div/div[2]/div/div[2]/div/div[1]").First().FindElements(SearchMethod.Tag, "a");
            foreach (var item in buttons)
            {
                if (item.GetTitle() == "Нравится")
                {
                    item.Click();
                }
            }
        }

        public bool IsBlockedCommunity()
        {
            return GetElementByXPath("/html/body/div").GetInnerText()
                .Contains("Данный материал заблокирован на территории Российской Федерации");
        }

        public void Authorization(string loginVK, string passwordVK)
        {
            OpenPageInNewTab("https://vk.com/");

            if (GetElementById("index_login") == null)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            var login = GetElementById("index_email");
            if (string.IsNullOrWhiteSpace(login.Text))
            {
                login.SendKeys(loginVK);
            }

            var password = GetElementById("index_pass");
            if (string.IsNullOrWhiteSpace(password.Text))
            {
                password.SendKeys(passwordVK);
            }

            GetElementById("index_login_button").Click();
            Thread.Sleep(2000);

            CloseTab();
            SwitchToTab();
            Thread.Sleep(1000);
        }

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
