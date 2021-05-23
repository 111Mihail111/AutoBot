using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Models;
using AutoBot.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoBot.Controllers
{
    public class StartController : Controller
    {
        private readonly IVLike _vLike;
        private readonly IVkTarget _vkTarget;
        private readonly IVkMyMarket _vkMyMarket;

        public StartController(IVLike vLike, IVkTarget vkTarget,IVkMyMarket vkMyMarket)
        {
            _vLike = vLike;
            _vkTarget = vkTarget;
            _vkMyMarket = vkMyMarket;
        }

        public ActionResult Index()
        {
            if (!AccountService.GetAccounts().Any())
            {
                string fileData = string.Empty;
                using (var stream = new StreamReader("../AutoBot/BrowserSettings/Учетки.txt", Encoding.Default))
                {
                    fileData = stream.ReadToEnd();
                };

                AccountManager accountManager = new();  //TODO:Прокинуть через DI
                accountManager.SaveAccounts(fileData);
            }

            return View(new WebSitesVM { InternetServices = new List<InternetService>(WebService.GetInternetServices()), });
        }

        /// <summary>
        /// Обновить таймер интернет-сервиса
        /// </summary>
        /// <param name="typeService">Тип сервиса</param>
        /// <param name="activationTime">Время активации</param>
        /// <returns>Результат частичного представления</returns>
        [HttpGet]
        public PartialViewResult InternetServiceTimerUpdate(TypeService typeService, TimeSpan activationTime)
        {
            activationTime -= TimeSpan.FromMinutes(1);
            if (activationTime < TimeSpan.FromSeconds(1))
            {
                activationTime = TimeSpan.FromSeconds(0);
            }

            WebService.UpdateTimerService(typeService, activationTime);

            return PartialView("_InternetService", WebService.GetInternetServices());
        }

        /// <summary>
        /// Обновить статус интернет-сервиса
        /// </summary>
        /// <param name="typeService">Тип сервиса</param>
        /// <param name="status">Статус сервиса</param>
        /// <param name="runType">Тип запуска сервиса</param>
        /// <returns>Результат частичного представления</returns>
        [HttpGet]
        public PartialViewResult InternetServiceStatusUpdate(TypeService typeService, Status status, RunType runType)
        {
            WebService.UpdateStatusService(typeService, status);

            var internetServices = WebService.GetInternetServices();
            if (runType == RunType.Manually)
            {
                return PartialView("_ManualStart", internetServices);
            }

            return PartialView("_InternetService", internetServices);
        }

        /// <summary>
        /// Перейти к интернет-сервису
        /// </summary>
        /// <param name="url">Интернет-адрес сервиса</param>
        /// <param name="typeService">Тип сервиса</param>
        /// <param name="runType">Тип запуска сервиса</param>
        /// <returns>Результат частичного представления</returns>
        [HttpGet]
        public PartialViewResult GoToInternetService(string url, TypeService typeService, RunType runType)
        {
            switch (typeService)
            {
                case TypeService.VLike:
                    _vLike.GoTo(url);
                    break;
                case TypeService.VkTarget:
                case TypeService.VkTarget_2:
                case TypeService.VkTarget_3:
                case TypeService.VkTarget_4:
                    _vkTarget.GoTo(url, typeService);
                    break;
                case TypeService.VkMyMarket:
                    _vkMyMarket.GoTo(url);
                    break;
            }

            var internetServices = WebService.GetInternetServices();
            if (runType == RunType.Auto)
            {
                return PartialView("_InternetService", internetServices);
            }

            return PartialView("_ManualStart", internetServices);
        }

        /// <summary>
        /// Пришло ли время запуска
        /// </summary>
        /// <param name="dateTimeLaunch">Дата и время запуска</param>
        /// <returns>True - пришло, иначе false</returns>
        [HttpGet]
        public bool IsTimeToLaunch(string dateTimeLaunch)
        {
            return DateTime.Now >= Convert.ToDateTime(dateTimeLaunch);
        }

        /// <summary>
        /// Получить представление с обновленными данными
        /// </summary>
        /// <returns>Результат частичного представления</returns>
        [HttpGet]
        public PartialViewResult UpdateDataManualStartView()
        {
            return PartialView("_ManualStart", WebService.GetInternetServices());
        }
    }
}