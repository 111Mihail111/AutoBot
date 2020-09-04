using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    public interface IClassmatesManager
    {
        public void Authorization();
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
