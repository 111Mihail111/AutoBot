﻿using AutoBot.Enums;
using AutoBot.Models;
using System;
using System.Collections.Generic;

namespace AutoBot.Area.Services
{
    public static class CraneService
    {
        public static List<Crane> _cranes = new List<Crane>
        {
            //new Crane { URL = "https://freebitco.in/", ActivityTime = TimeSpan.FromHours(0), BalanceOnCrane = "0",
            //    StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.Bitcoin, TypeCrane = TypeCrane.FreeBitcoin },
            new Crane { URL = "https://moonbit.co.in/faucet", ActivityTime = TimeSpan.FromMinutes(0), BalanceOnCrane = "0",
                StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.Bitcoin, TypeCrane = TypeCrane.MoonBitcoin },
            //new Crane { URL = "https://moondoge.co.in/faucet", ActivityTime = TimeSpan.FromMinutes(0), BalanceOnCrane = "0",
            //    StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.Dogecoin, TypeCrane = TypeCrane.MoonDogecoin},
            //new Crane { URL = "http://bonusbitcoin.co/faucet", ActivityTime = TimeSpan.FromHours(0), BalanceOnCrane = "0",
            //    StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.Bitcoin, TypeCrane = TypeCrane.BonusBitcoin },
            //new Crane { URL = "https://moonliteco.in/faucet", ActivityTime = TimeSpan.FromHours(0), BalanceOnCrane = "0",
            //    StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.LiteCoin, TypeCrane = TypeCrane.MoonLitecoin, },
            //new Crane { URL = "https://moondash.co.in/faucet", ActivityTime = TimeSpan.FromHours(0), BalanceOnCrane = "0",
            //    StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.Dash, TypeCrane = TypeCrane.MoonDash, }
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