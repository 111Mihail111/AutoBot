using AutoBot.Area.Managers.Interface;
using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers
{
    public class Reddit : BrowserManager, IReddit
    {

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
