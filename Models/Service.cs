namespace AutoBot.Models
{
    /// <summary>
    /// Модель интернет-сервиса
    /// </summary>
    public class Service
    {
        /// <summary>
        /// Url-адрес сервиса
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Баланс на сервисе
        /// </summary>
        public double BalanceOnService { get; set; }
    }
}
