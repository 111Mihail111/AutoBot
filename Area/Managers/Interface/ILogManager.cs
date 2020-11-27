using System;

namespace AutoBot.Area.Managers.Interface
{
    /// <summary>
    /// Менеджер логирования ошибок
    /// </summary>
    public interface ILogManager
    {
        /// <summary>
        /// Сохранить детали ошибки
        /// </summary>
        /// <param name="exception">Исключение</param>
        public void SaveDetailsException(Exception exception);
    }
}
