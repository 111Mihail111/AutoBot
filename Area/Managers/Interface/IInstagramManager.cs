using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    public interface IInstagramManager
    {
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void Authorization(string login, string password);
        /// <summary>
        /// Подписаться на пользователя
        /// </summary>
        public void Subscribe();
        /// <summary>
        /// Отписаться от пользователя
        /// </summary>
        public void Unsubscribe();
        /// <summary>
        /// Поставить лайк
        /// </summary>
        public void PutLike();
        /// <summary>
        /// Удалить лайк
        /// </summary>
        public void RemoveLike();
        /// <summary>
        /// Найдена ли страница
        /// </summary>
        /// <returns>True - найдена, иначе false</returns>
        public bool IsFoundPage();
        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
