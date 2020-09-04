using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    public interface IVkManager
    {
        /// <summary>
        /// Вступить в сообщество
        /// </summary>
        public void JoinToComunity();

        /// <summary>
        /// Поставить лайк
        /// </summary>
        public void PutLike();

        /// <summary>
        /// Заблокировано ли сообщество
        /// </summary>
        /// <returns></returns>
        public bool IsBlockedCommunity();

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="loginVK">Логин вк</param>
        /// <param name="passwordVK">Пароль вк</param>
        public void Authorization(string loginVK, string passwordVK);

        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
