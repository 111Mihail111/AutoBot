using System;

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
        /// <param name="exception">Возникшее исключение</param>
        /// <param name="base64Encoded">Изображение в base64</param>
        public void SendToEmail(Exception exception, string base64Encoded);
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="taskDescription">Описание задачи</param>
        /// <param name="methodName">Наименования метода</param>
        /// <param name="url">Url-адрес страницы</param>
        public void SendToEmail(string taskDescription, string methodName, string url);
    }
}
