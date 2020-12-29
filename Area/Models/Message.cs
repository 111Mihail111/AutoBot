using System;

namespace AutoBot.Area.Models
{
    /// <summary>
    /// Сообщение
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Исключение
        /// </summary>
        public Exception Exception { get; set; }
        /// <summary>
        /// Изображение в base64
        /// </summary>
        public string Base64Encoded { get; set; }
        /// <summary>
        /// Url-адрес
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Тема
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// Количество вкладок
        /// </summary>
        public int TabsCount { get; set; }
    }
}
