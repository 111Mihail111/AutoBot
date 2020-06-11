using AutoBot.Enums;
using AutoBot.Models;
using System;
using System.Collections.Generic;

namespace AutoBot.Area.Service
{
    public static class CraneService
    {
        public static List<Crane> _cranes = new List<Crane>
            {
                new Crane { URL = "https://freebitco.in/", ActivityTime = TimeSpan.FromHours(1),
                    BalanceOnCrane = 0, StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.Bitcoin, TypeCrane = TypeCrane.FreeBitcoin },
            };

        /// <summary>
        /// Получить краны
        /// </summary>
        /// <returns>Лист кранов</returns>
        public static IEnumerable<Crane> GetCranes()
        {
            return _cranes;
        }

        /// <summary>
        /// Обновить кран
        /// </summary>
        /// <param name="crane">Модель крана</param>
        public static void UpdateCrane(Crane crane)
        {
            int index = _cranes.FindIndex(fi => fi.URL == crane.URL);
            _cranes[index] = crane;
        }

    }
}
