using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    /// <summary>
    /// VK менеджер
    /// </summary>
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
        public void RemoveLike();

        /// <summary>
        /// Заблокировано ли сообщество
        /// </summary>
        /// <returns>True - да, иначе нет</returns>
        public bool IsBlockedCommunity();

        /// <summary>
        /// Приватное ли сообщество
        /// </summary>
        /// <returns>True - да, иначе false</returns>
        public bool IsPrivateGroup();

        /// <summary>
        /// Найден ли пост
        /// </summary>
        /// <returns>True - найден, иначе false</returns>
        public bool IsPostFound();

        /// <summary>
        /// Рассказать о группе
        /// </summary>
        public void ToTellAboutGroup();

        /// <summary>
        /// Добавить в друзья
        /// </summary>
        public void AddToFriends();

        /// <summary>
        /// Удалить из друзей
        /// </summary>
        public void RemoveFromFriends();

        /// <summary>
        /// Заблокирован ли аккаунт
        /// </summary>
        /// <returns>True если аккаунт заблокирован, иначе false</returns>
        public bool IsBlockedAccount();

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="loginVK">Логин вк</param>
        /// <param name="passwordVK">Пароль вк</param>
        public void Authorization(string loginVK, string passwordVK);

        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
    }
}
