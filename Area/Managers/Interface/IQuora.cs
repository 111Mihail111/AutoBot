using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    public interface IQuora
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
        /// Лайкнуть ответ
        /// </summary>
        public void LikeAnswer();
        /// <summary>
        /// Удалить лайк
        /// </summary>
        public void RemoveLike();
        /// <summary>
        /// Сделать репост
        /// </summary>
        public void MakeRepost();
        /// <summary>
        /// Является ли страница профилем пользователя
        /// </summary>
        /// <returns>True - да, иначе False</returns>
        public bool IsUserPageProfile();
        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
