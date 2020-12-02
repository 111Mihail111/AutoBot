using AutoBot.Area.Managers.Interface;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace AutoBot.Area.Managers
{
    public class LogManager : BrowserManager, ILogManager
    {
        /// <summary>
        /// Email отправителя
        /// </summary>
        private readonly MailAddress _mailFrom = new MailAddress("desiptikon.bot@yandex.ru");
        /// <summary>
        /// Email получателя
        /// </summary>
        private readonly MailAddress _mailTo = new MailAddress("polowinckin.mixail@yandex.ru");


        /// <inheritdoc/>
        public void SendToEmail(Exception exception, string base64Encoded)
        {
            var description = GetDescriptionException(exception);
            var imageTag = $"<img src='data:image/png;base64,{base64Encoded}' style='width:100%; height:100%;'>";

            var message = new MailMessage(_mailFrom, _mailTo)
            {
                Subject = "Возникла ошибка",
                IsBodyHtml = true,
                Body = $"{description}<br><center><h3>Изображение страницы</h3>{imageTag}</center>",
            };

            Send(message);
        }
        /// <inheritdoc/>
        public void SendToEmail(string taskDescription, string methodName, string url)
        {
            var message = new MailMessage(_mailFrom, _mailTo)
            {
                Subject = "Новая задача",
                IsBodyHtml = true,
                Body = $"<b>Тип</b>: {taskDescription}" +
                       $"<br><b>Метод</b>: {methodName}" +
                       $"<br><b>Url-адрес</b>: {url}",
            };

            Send(message);
        }


        /// <summary>
        /// Получить описание исключения
        /// </summary>
        /// <param name="exception">Возникшее исключение</param>
        /// <returns>Описание исключения</returns>
        protected string GetDescriptionException(Exception exception)
        {
            var stackTrace = new StackTrace(exception, true);
            var errorStack = stackTrace.GetFrames().First();

            return $"<b>Файл</b>: {errorStack.GetFileName()}" +
                   $"<br><b>Строка</b>: {errorStack.GetFileLineNumber()}" +
                   $"<br><b>Столбец</b>: {errorStack.GetFileColumnNumber()}" +
                   $"<br><b>Метод</b>: {errorStack.GetMethod()}";
        }
        /// <summary>
        /// Отправить
        /// </summary>
        protected void Send(MailMessage mailMessage)
        {
            var smtpClient = new SmtpClient("smtp.yandex.ru", 587)
            {
                Credentials = new NetworkCredential("desiptikon.bot@yandex.ru", "123q_Q*W(*E&*R^*Z$*X!*C?*V123q_Q*W(*E&*R^*Z$*X!*C?*V"),
                EnableSsl = true,
            };

            smtpClient.Send(mailMessage);
        }
    }
}
