using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    /// <summary>
    /// Тамблер
    /// </summary>
    interface ITumblr
    {
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void Authorization(string login, string password);
        /// <summary>
        /// Реблог поста
        /// </summary>
        public void Reblog();
        /// <summary>
        /// Лайкнуть пост
        /// </summary>
        public void LikePost();
        /// <summary>
        /// Удалить лайк
        /// </summary>
        public void RemoveLike();
        /// <summary>
        /// Подписаться на блог
        /// </summary>
        public void SubscribeToBlog();
        /// <summary>
        /// Отписаться от блога
        /// </summary>
        public void UnsubscribeToBlog();
        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
