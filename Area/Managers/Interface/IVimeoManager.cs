using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    public interface IVimeoManager
    {
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void Authorization(string login, string password);
        /// <summary>
        /// Авторизация в фоновом режиме браузера
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        public void AuthorizationInBrowserBackground(string login, string password);
        /// <summary>
        /// Лайк под видео
        /// </summary>
        public void LikeUnderVideo();
        /// <summary>
        /// Удалить лайк
        /// </summary>
        public void RemoveLike();
        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
