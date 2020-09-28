using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    /// <summary>
    /// Ютуб менеджер
    /// </summary>
    public interface IYouTubeManager
    {
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void Authorization(string login, string password);

        /// <summary>
        /// Подписаться на канал
        /// </summary>
        public void SubscribeToChannel();

        /// <summary>
        /// Лайк под видео
        /// </summary>
        public void LikeUnderVideo();

        /// <summary>
        /// Дизлайк под видео
        /// </summary>
        public void DislikeUnderVideo();

        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
