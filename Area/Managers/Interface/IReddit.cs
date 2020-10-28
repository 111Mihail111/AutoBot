using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    /// <summary>
    /// Рэдит
    /// </summary>
    interface IReddit
    {
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void Authorization(string login, string password);
        /// <summary>
        /// Стрелка вверх под постом
        /// </summary>
        public void UpArrowForPost();
        /// <summary>
        /// Стрелка вниз под постом
        /// </summary>
        public void DownArrowUnderPost();
        /// <summary>
        /// Подписаться на пользователя
        /// </summary>
        public void Subscribe();
        /// <summary>
        /// Отписаться от пользователя
        /// </summary>
        public void Unsubscribe();
        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
