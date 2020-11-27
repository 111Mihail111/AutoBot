using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.Managers.Interface
{
    /// <summary>
    /// Однокласники менеджер
    /// </summary>
    public interface IClassmatesManager
    {
        /// <summary>
        /// Авторизация через почту
        /// </summary>
        /// <param name="email">Email-адрес</param>
        /// <param name="password">Пароль к почте</param>
        public void AuthorizationThroughMail(string email, string password);
        /// <summary>
        /// Вступить в группу
        /// </summary>
        public void JoinGroup();
        /// <summary>
        /// Покинуть группу
        /// </summary>
        public void LeaveGroup();
        /// <summary>
        /// Поставить класс
        /// </summary>
        public void PutClass();
        /// <summary>
        /// Удалить класс
        /// </summary>
        public void RemoveClass();
        /// <summary>
        /// Сделать репост
        /// </summary>
        public void MakeRepost();
        /// <summary>
        /// Добавить в друзья
        /// </summary>
        public void AddToFriends();
        /// <summary>
        /// Считать контекст браузер менеджера
        /// </summary>
        /// <param name="chromeDriver">Хром драйвер</param>
        public void SetContextBrowserManager(ChromeDriver chromeDriver);
        /// <summary>
        /// Заблокирован ли контент
        /// </summary>
        /// <returns>True - заблокирован, иначе false</returns>
        public bool IsBlokedContent();
    }
}
