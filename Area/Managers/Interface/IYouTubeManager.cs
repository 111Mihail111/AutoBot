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
        /// Авторизация под старую версию браузера
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void AuthorizationForOldVersionBrowser(string login, string password);
        /// <summary>
        /// Подписаться на канал
        /// </summary>
        public void SubscribeToChannel();
        /// <summary>
        /// Отписаться от канала
        /// </summary>
        public void UnsubscribeFromChannel();
        /// <summary>
        /// Лайк под видео
        /// </summary>
        public void LikeUnderVideo();
        /// <summary>
        /// Удалить лайк под видео
        /// </summary>
        public void RemoveLike();
        /// <summary>
        /// Дизлайк под видео
        /// </summary>
        public void DislikeUnderVideo();
        /// <summary>
        /// Удалить дизлайк под видео
        /// </summary>
        public void RemoveDislike();
        /// <summary>
        /// Доступно ли видео
        /// </summary>
        /// <returns>True - доступно, иначе false</returns>
        public bool IsVideoAvailable();
        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
