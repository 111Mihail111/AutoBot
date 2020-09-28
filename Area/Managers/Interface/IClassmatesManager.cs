using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    /// <summary>
    /// Однокласники менеджер
    /// </summary>
    public interface IClassmatesManager
    {
        /// <summary>
        /// Авторизация через почту
        /// </summary>
        /// <param name="email">Email-адрес</param>
        /// <param name="password">Пароль к почте</param>
        public void AuthorizationThroughMail(string email, string password);

        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
