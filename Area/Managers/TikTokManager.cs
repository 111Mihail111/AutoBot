using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class TikTokManager : BrowserManager, ITikTokManager
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            OpenPageInNewTab("https://www.tiktok.com/");

            var button = GetElementByClassName("login-button");
            if (button == null)
            {
                CloseTab();
                SwitchToTab();
                return;
            }
            button.Click();

            SwitchToFrame(GetElementByTagName("iframe"));
            
            var divCollection = GetElementsByClassName("channel-item-wrapper-2gBWB");
            foreach (var div in divCollection)
            {
                string action = div.GetInnerText();
                if (action != "Войти через VK")
                {
                    continue;
                }

                div.Click();
                Thread.Sleep(3000);
                break;
            }
            
            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void Subscribe()
        {
            GetElementsByClassName("follow-button").Where(w => w.GetInnerText() == "Подписаться").FirstOrDefault().Click();
            Thread.Sleep(2500);
        }
        /// <inheritdoc/>
        public void Unsubscribe()
        {
            GetElementsByClassName("follow-button").Where(w => w.GetInnerText() == "Подписки").FirstOrDefault().Click();
            Thread.Sleep(2500);
        }
        /// <inheritdoc/>
        public void PutLike()
        {
            var divElement = GetElementByClassName("video-feed-container").FindElement(SearchMethod.XPath, "/div/div[1]");
            var divLikeElement = divElement.FindElement(SearchMethod.ClassName, "engagement-icon");
            var fillColor = divLikeElement.FindElement(SearchMethod.Tag, "svg").GetFill();

            if (fillColor != "currentColor")
            {
                return;
            }

            divLikeElement.Click();
            Thread.Sleep(2500);
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
