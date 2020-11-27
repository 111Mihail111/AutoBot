using AutoBot.Area.Managers.Interface;
using System;
using System.Diagnostics;

namespace AutoBot.Area.Managers
{
    public class LogManager : BrowserManager, ILogManager
    {
        /// <inheritdoc/>
        public void SaveDetailsException(Exception exception)
        {
            var description = GetDescriptionException(exception);
        }

        private string GetDescriptionException(Exception exception)
        {
            var trace = new StackTrace(exception, true);

            return "Детали ошибки:" +
                   $"1) Произошло нажатие на элемент, который был равен null ({exception.Message})." +
                   $"2) Ошибка возникла в строке №{""} метода {""}.";
        }
    }
}
