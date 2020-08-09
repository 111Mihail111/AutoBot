using AutoBot.Area.Enums;
using AutoBot.Models;
using AutoBot.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoBot.Area.Services
{
    public static class WebService
    {
        /// <summary>
        /// Краны
        /// </summary>
        public static List<Crane> _cranes = new List<Crane>
        {
            new Crane { URL = "https://freebitco.in/", ActivityTime = TimeSpan.FromHours(0), BalanceOnCrane = "0",
                StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.Bitcoin, TypeCrane = TypeCrane.FreeBitcoin },
            //new Crane { URL = "https://moonbit.co.in/faucet", ActivityTime = TimeSpan.FromMinutes(0), BalanceOnCrane = "0",
            //    StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.Bitcoin, TypeCrane = TypeCrane.MoonBitcoin },
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
        /// Интернет-сервисы
        /// </summary>
        public static List<InternetService> _services = new List<InternetService>
        {
            //new InternetService { URL = "https://v-like.ru/", ActivityTime = TimeSpan.FromHours(0), BalanceOnService = "0",
            //    StatusService = Status.Work, TypeService = TypeService.V_Like },
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
        /// Получить интернет сервисы
        /// </summary>
        /// <returns>Лист интернет сервисов</returns>
        public static IEnumerable<InternetService> GetInternetServices()
        {
            return _services;
        }
        /// <summary>
        /// Получить все данные
        /// </summary>
        /// <returns></returns>
        public static WebSitesVM GetAllData()
        {
            return new WebSitesVM { Cranes = GetCranes().ToList(), InternetServices = GetInternetServices().ToList() };
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

        /// <summary>
        /// Обновить статус крана
        /// </summary>
        /// <param name="url">Url-адрес крана</param>
        /// <param name="status">Статус крана</param>
        public static void UpdateStatusCrane(string url, Status status)
        {
            int index = _cranes.FindIndex(fi => fi.URL == url);
            _cranes[index].StatusCrane = status;
        }

        /// <summary>
        /// Обновить интернет-сервис
        /// </summary>
        /// <param name="internetService">Модель интернет сервиса</param>
        public static void UpdateInternetService(InternetService internetService)
        {
            int index = _services.FindIndex(fi => fi.URL == internetService.URL);
            _services[index] = internetService;
        }
    }
}
