﻿using AutoBot.Area.Enums;
using System;

namespace AutoBot.Models
{
    /// <summary>
    /// Модель интернет-сервиса
    /// </summary>
    public class InternetService
    {
        /// <summary>
        /// Url-адрес сервиса
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// Время активации
        /// </summary>
        public TimeSpan ActivationTime { get; set; }
        /// <summary>
        /// Статус сервиса
        /// </summary>
        public Status StatusService { get; set; }
        /// <summary>
        /// Мой баланс на сервесе
        /// </summary>
        public string BalanceOnService { get; set; }
        /// <summary>
        /// Тип сервиса
        /// </summary>
        public TypeService TypeService { get; set; }
        /// <summary>
        /// Тип запуска
        /// </summary>
        public RunType RunType { get; set; }
        /// <summary>
        /// Время запуска
        /// </summary>
        public DateTime? LaunchTime { get; set; }
    }
}