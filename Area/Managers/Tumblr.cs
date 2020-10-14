using AutoBot.Area.Managers.Interface;
using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers
{
    public class Tumblr : BrowserManager, ITumblr
    {
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
