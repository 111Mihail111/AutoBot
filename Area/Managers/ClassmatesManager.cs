using AutoBot.Area.Managers.Interface;
using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers
{
    public class ClassmatesManager : BrowserManager, IClassmatesManager
    {
        public void Authorization()
        {

        }

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
