﻿using OpenQA.Selenium;
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
        /// <param name="url">Url-адрес страницы</param>
        void SendToEmail(Exception exception, string base64Encoded, string url);
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="webDriverException">Возникшее WebDriverException-исключение</param>
        /// <param name="base64Encoded">Изображение в base64</param>
        /// <param name="url">Url-адрес страницы</param>
        void SendToEmail(WebDriverException webDriverException, string base64Encoded, string url);
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="nullReferenceException">Возникшее NullReferenceException-исключение</param>
        /// <param name="base64Encoded">Изображение в base64</param>
        /// <param name="url">Url-адрес страницы</param>
        void SendToEmail(NullReferenceException nullReferenceException, string base64Encoded, string url);
        /// <summary>
        /// Отправить сообщение
        /// </summary>
        /// <param name="taskDescription">Описание задачи</param>
        /// <param name="methodName">Наименования метода</param>
        /// <param name="url">Url-адрес страницы</param>
        void SendToEmail(string taskDescription, string methodName, string url);
    }
}
