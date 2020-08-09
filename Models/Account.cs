namespace AutoBot.Models
{
    /// <summary>
    /// Аккаунт
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Тип веб-сайта
        /// </summary>
        public string TypeWebSite { get; set; }
        /// <summary>
        /// API ключ
        /// </summary>
        public string ApiKey { get; set; }
    }
}
