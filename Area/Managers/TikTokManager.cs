using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;

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
            button.ToClick();

            SwitchToFrame(GetElementByTagName("iframe"));
            
            var divCollection = GetElementsByClassName("channel-item-wrapper-2gBWB");
            foreach (var div in divCollection)
            {
                string action = div.GetInnerText();
                if (action != "Войти через VK")
                {
                    continue;
                }

                div.ToClick(3000);
                break;
            }
            
            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void Subscribe()
        {
            int counter = 0;
            while (counter != 2)
            {
                var button = GetElementsByClassName("follow-button").Where(w => w.GetInnerText() == "Подписаться").FirstOrDefault();
                if (button == null)
                {
                    return;
                }
                
                button.ToClick(5000);
                counter++;

                RefreshPage();
            }
        }
        /// <inheritdoc/>
        public void Unsubscribe()
        {
            int counter = 0;
            while (counter != 2)
            {
                var button = GetElementsByClassName("follow-button").Where(w => w.GetInnerText() == "Подписки").FirstOrDefault();
                if (button == null)
                {
                    return;
                }

                button.ToClick(5000);
                counter++;

                RefreshPage();
            }
        }
        /// <inheritdoc/>
        public void PutLike()
        {
            int counter = 0;
            while (counter != 2)
            {
                var divElement = GetElementByClassName("video-feed-container").FindElement(SearchMethod.XPath, "span/div/div[1]");
                var divLikeElement = divElement.FindElement(SearchMethod.ClassName, "engagement-icon");
                var fillColor = divLikeElement.FindElement(SearchMethod.Tag, "svg").GetFill();

                if (fillColor != "currentColor")
                {
                    return;
                }

                divLikeElement.ToClick(5000);
                counter++;

                RefreshPage();
            }
        }
        /// <inheritdoc/>
        public void RemoveLike()
        {
            int counter = 0;
            while (counter != 2)
            {
                var divElement = GetElementByClassName("video-feed-container").FindElement(SearchMethod.XPath, "span/div/div[1]");
                var divLikeElement = divElement.FindElement(SearchMethod.ClassName, "engagement-icon");
                var fillColor = divLikeElement.FindElement(SearchMethod.Tag, "svg").GetFill();

                if (fillColor != "currentColor")
                {
                    return;
                }

                divLikeElement.ToClick(5000);
                counter++;

                RefreshPage();
            }
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
