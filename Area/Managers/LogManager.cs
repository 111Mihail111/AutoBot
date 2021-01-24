using AutoBot.Area.Managers.Interface;
using AutoBot.Area.Models;
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
    public class LogManager : ILogManager
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
        public void SendToEmail(string taskDescription, string methodName, string url, string topic)
        {
            var message = new MailMessage(_mailFrom, _mailTo)
            {
                Subject = topic,
                IsBodyHtml = true,
                Body = $"<b>Тип</b>: {taskDescription}" +
                       $"<br><b>Метод</b>: {methodName}" +
                       $"<br><b>Url-адрес</b>: {url}",
            };

            Send(message);
        }
        /// <inheritdoc/>
        public void SendToEmail(Message message)
        {
            var description = GetDetailsException(message); 
            var imageTag = $"<img src='data:image/png;base64,{message.Base64Encoded}' style='width:100%; height:100%;'>";

            var mailMessage = new MailMessage(_mailFrom, _mailTo)
            {
                Subject = message.Topic,
                IsBodyHtml = true,
                Body = $"{description}<br><center><h3>Изображение страницы</h3>{imageTag}</center>",
            };

            Send(mailMessage);
        }


        /// <summary>
        /// Получить описание исключения
        /// </summary>
        /// <param name="exception">Возникшее исключение</param>
        /// <returns>Описание исключения</returns>
        protected StringBuilder GetDetailsException(Message message)
        {
            var stringBuilder = new StringBuilder();
            if (message.Exception == null)
            {
                return stringBuilder;
            }
            
            var stackTrace = new StackTrace(message.Exception, true);

            foreach (var errorStack in stackTrace.GetFrames().Where(w => w.GetFileLineNumber() != 0))
            {
                stringBuilder.AppendLine($"<br><br><b>Файл</b>: {errorStack.GetFileName()}");
                stringBuilder.AppendLine($"<br><b>Строка</b>: {errorStack.GetFileLineNumber()}");
                stringBuilder.AppendLine($"<br><b>Столбец</b>: {errorStack.GetFileColumnNumber()}");
                stringBuilder.AppendLine($"<br><b>Метод</b>: {errorStack.GetMethod()}");
            }

            stringBuilder.AppendLine($"<br><b>Описание</b>: {message.Exception.Message}");
            stringBuilder.AppendLine($"<br><b>Кол-во. вкладок</b>: {message.TabsCount}");
            stringBuilder.AppendLine($"<br><b>Url-Адрес</b>: {message.Url}");

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
