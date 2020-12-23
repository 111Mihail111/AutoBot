using AutoBot.Area.Managers.Interface;
using OpenQA.Selenium;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

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
        /// <inheritdoc/>
        public void SendToEmail(string taskDescription, string methodName, string url, string base64Encoded)
        {
            var imageTag = $"<img src='data:image/png;base64,{base64Encoded}' style='width:100%; height:100%;'>";

            var message = new MailMessage(_mailFrom, _mailTo)
            {
                Subject = "Новая задача",
                IsBodyHtml = true,
                Body = $"<b>Тип</b>: {taskDescription}" +
                       $"<br><b>Метод</b>: {methodName}" +
                       $"<br><b>Url-адрес</b>: {url}" +
                       $"<br><center><h3>Изображение страницы</h3>{imageTag}</center>",
            };

            Send(message);
        }
        /// <inheritdoc/>
        public void SendToEmail(Exception exception, string base64Encoded, string url)
        {
            var description = GetDescriptionException(exception) + $"<br><b>Url-Адрес</b>: {url}";
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
        public void SendToEmail(Exception exception, string base64Encoded, string url, string topic)
        {
            var description = GetDescriptionException(exception) + $"<br><b>Url-Адрес</b>: {url}";
            var imageTag = $"<img src='data:image/png;base64,{base64Encoded}' style='width:100%; height:100%;'>";

            var message = new MailMessage(_mailFrom, _mailTo)
            {
                Subject = topic,
                IsBodyHtml = true,
                Body = $"{description}<br><center><h3>Изображение страницы</h3>{imageTag}</center>",
            };

            Send(message);
        }


        /// <summary>
        /// Получить описание исключения
        /// </summary>
        /// <param name="exception">Возникшее исключение</param>
        /// <returns>Описание исключения</returns>
        protected StringBuilder GetDescriptionException(Exception exception)
        {
            var stringBuilder = new StringBuilder();
            var stackTrace = new StackTrace(exception, true);

            foreach (var errorStack in stackTrace.GetFrames().Where(w => w.GetFileLineNumber() != 0))
            {
                stringBuilder.AppendLine($"<br><br><b>Файл</b>: {errorStack.GetFileName()}");
                stringBuilder.AppendLine($"<br><b>Строка</b>: {errorStack.GetFileLineNumber()}");
                stringBuilder.AppendLine($"<br><b>Столбец</b>: {errorStack.GetFileColumnNumber()}");
                stringBuilder.AppendLine($"<br><b>Метод</b>: {errorStack.GetMethod()}");
            }

            stringBuilder.AppendLine($"<br><b>Описание</b>: {exception.Message}");
            stringBuilder.AppendLine($"<br><b>Кол-во. вкладок</b>: {GetTabsCount()}");

            return stringBuilder;
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
