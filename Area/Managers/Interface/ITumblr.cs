using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    /// <summary>
    /// Тамблер менеджер
    /// </summary>
    interface ITumblr
    {
        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
