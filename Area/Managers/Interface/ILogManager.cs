using AutoBot.Area.Models;

namespace AutoBot.Area.Managers.Interface
{
    /// <summary>
    /// Менеджер логирования ошибок
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="taskDescription">Описание задачи</param>
        /// <param name="methodName">Наименования метода</param>
        /// <param name="url">Url-адрес страницы</param>
        /// <param name="topic">Тема письма</param>
        public void SendToEmail(string taskDescription, string methodName, string url, string topic);
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="message">Модель сообщения</param>
        void SendToEmail(Message message);
    }
}
