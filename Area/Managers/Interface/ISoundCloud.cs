using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    public interface ISoundCloud
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
        /// Лайкнуть трэк
        /// </summary>
        public void LikeTrack();
        /// <summary>
        /// Удалить лайк
        /// </summary>
        public void RemoveLike();
        /// <summary>
        /// Поделиться трэком
        /// </summary>
        public void RepostTrack();
        /// <summary>
        /// Удалить репост
        /// </summary>
        public void RemoveRepost();
        /// <summary>
        /// Скачать трэк
        /// </summary>
        public void DownloadTrack();
        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
