using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class YouTubeManager : BrowserManager, IYouTubeManager
    {
        public void Authorization(string login, string password)
        {
            OpenPageInNewTab("https://www.youtube.com/");

            if (GetElementById("avatar-btn") != null)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            GetElementById("buttons").FindElement(SearchMethod.ClassName, "style-scope ytd-masthead style-suggestive size-small")
                .FindElement(SearchMethod.Tag, "a").Click();
            GetElementById("identifierId").SendKeys(login);
            GetElementById("identifierNext").FindElement(SearchMethod.Tag, "button").Click();
            GetElementById("password").FindElement(SearchMethod.Tag, "input").SendKeys(password);
            GetElementById("passwordNext").FindElement(SearchMethod.Tag, "button").Click();
        }

        public void SubscribeToChannel()
        {
            GetElementsByClassName("style-scope ytd-subscribe-button-renderer").First().Click();
            Thread.Sleep(1500);
        }

        public void LikeUnderVideo()
        {
            GetElementById("top-level-buttons").FindElements(SearchMethod.Tag, "button").First().Click();
            Thread.Sleep(1500);
        }

        public void DislikeUnderVideo()
        {
            GetElementById("top-level-buttons").FindElements(SearchMethod.Tag, "button")
                .Where(w => w.GetAttribute("aria-pressed") == "false").Last().Click();
            Thread.Sleep(1500);
        }

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            GetDriver(chromeDriver);
        }
    }
}
