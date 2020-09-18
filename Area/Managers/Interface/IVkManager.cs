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
        /// Выйти из сообщества
        /// </summary>
        public void UnsubscribeToComunity();

        /// <summary>
        /// Поставить лайк
        /// </summary>
        public void PutLike();

        /// <summary>
        /// Сделать репост
        /// </summary>
        public void MakeRepost();

        /// <summary>
        /// Убрать лайк
        /// </summary>
        /// <param name="url">Url-адрес страницы с постом</param>
        public void RemoveLike(string url);

        /// <summary>
        /// Заблокировано ли сообщество
        /// </summary>
        /// <returns>True - да, иначе нет</returns>
        public bool IsBlockedCommunity();

        /// <summary>
        /// Репост записи
        /// </summary>
        public void RepostEntries();

        /// <summary>
        /// Приватное ли сообщество
        /// </summary>
        /// <returns>True - да, иначе false</returns>
        public bool IsPrivateGroup();

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="loginVK">Логин вк</param>
        /// <param name="passwordVK">Пароль вк</param>
        public void Authorization(string loginVK, string passwordVK);

        /// <summary>
        /// Удаление модальных окон vk
        /// </summary>
        public void RemoveWindowMessage();

        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
