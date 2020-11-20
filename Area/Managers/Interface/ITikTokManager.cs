using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    public interface ITikTokManager
    {
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void Authorization(string login, string password);
        /// <summary>
        /// Подписаться
        /// </summary>
        public void Subscribe();
        /// <summary>
        /// Отписаться
        /// </summary>
        public void Unsubscribe();
        /// <summary>
        /// Поставить лайк
        /// </summary>
        public void PutLike();
        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
