using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    public interface IYandexZenManager
    {
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void Authorization(string login, string password);

        /// <summary>
        /// Поставить лайк
        /// </summary>
        public void PutLike();

        /// <summary>
        /// Удалить лайк
        /// </summary>
        public void RemoveLike();

        /// <summary>
        /// Подписаться
        /// </summary>
        public void Subscribe();

        /// <summary>
        /// Отписаться
        /// </summary>
        public void Unsubscribe();

        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
