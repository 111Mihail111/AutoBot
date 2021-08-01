using AutoBot.Area.Enums;
using AutoBot.Models;
using System;
using System.Collections.Generic;

namespace AutoBot.Area.Services
{
    public static class WebService
    {
        /// <summary>
        /// Интернет-сервисы
        /// </summary>
        private static List<InternetService> _services = new()
        {
            new InternetService 
            { 
                URL = "https://v-like.ru/", BalanceOnService = "0", StatusService = Status.NoWork, 
                TypeService = TypeService.VLike, RunType = RunType.Manually 
            },
            new InternetService 
            { 
                URL="https://vktarget.ru/", BalanceOnService = "0", StatusService = Status.NoWork,
                TypeService = TypeService.VkTarget, RunType = RunType.Manually 
            },
            new InternetService 
            { 
                URL="https://vktarget.ru/", BalanceOnService = "0", StatusService = Status.NoWork,
                TypeService = TypeService.VkTarget_2, RunType = RunType.Manually 
            },
            new InternetService 
            { 
                URL="https://vktarget.ru/", BalanceOnService = "0", StatusService = Status.NoWork,
                TypeService = TypeService.VkTarget_3, RunType = RunType.Manually 
            },
            new InternetService 
            { 
                URL="https://vktarget.ru/", BalanceOnService = "0", StatusService = Status.NoWork,
                TypeService = TypeService.VkTarget_4, RunType = RunType.Manually 
            },
            new InternetService 
            { 
                URL = "http://vkmymarket.ru/", BalanceOnService = "0", StatusService = Status.NoWork, 
                TypeService = TypeService.VkMyMarket, RunType = RunType.Manually 
            },
        };

        /// <summary>
        /// Получить Интернет-сервисы
        /// </summary>
        /// <returns>Коллекция Интернет-сервисов</returns>
        public static IEnumerable<InternetService> GetInternetServices()
        {
            return _services;
        }
        /// <summary>
        /// Обновить статус Интернет-сервиса
        /// </summary>
        /// <param name="typeService">Тип сервиса</param>
        /// <param name="status">Статус сервиса</param>
        public static void UpdateStatusService(TypeService typeService, Status status)
        {
            int index = _services.FindIndex(fi => fi.TypeService == typeService);
            _services[index].StatusService = status;
        }
        /// <summary>
        /// Обновить таймер Интернет-сервиса
        /// </summary>
        /// <param name="typeService">Тип сервиса</param>
        /// <param name="activationTime">Время активации</param>
        public static void UpdateTimerService(TypeService typeService, TimeSpan activationTime)
        {
            int index = _services.FindIndex(fi => fi.TypeService == typeService);
            _services[index].ActivationTime = activationTime;
        }
        /// <summary>
        /// Обновить Интернет-сервис
        /// </summary>
        /// <param name="internetService">Модель интернет сервиса</param>
        public static void UpdateInternetService(InternetService internetService)
        {
            int index = _services.FindIndex(fi => fi.TypeService == internetService.TypeService);
            _services[index] = internetService;
        }
    }
}