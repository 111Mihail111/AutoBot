using AutoBot.Enums;
using System;

namespace AutoBot.Models
{
    /// <summary>
    /// Кран
    /// </summary>
    public class Crane
    {
        /// <summary>
        /// Ссылка
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// Время активности
        /// </summary>
        public TimeSpan ActivityTime { get; set; }
        /// <summary>
        /// Статус Крана
        /// </summary>
        public Status StatusCrane { get; set; }
        /// <summary>
        /// Мой баланс на кране
        /// </summary>
        public string BalanceOnCrane { get; set; }
        /// <summary>
        /// Тип валюты
        /// </summary>
        public TypeCurrencies TypeCurrencies { get; set; }
        /// <summary>
        /// Тип крана
        /// </summary>
        public TypeCrane TypeCrane { get; set; }
    }
}
