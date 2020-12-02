using AutoBot.Area.CollectingСryptocurrencies.Interface;
using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AutoBot.Controllers
{
    public class StartController : Controller
    {
        private readonly IFreeBitcoin _freeBitcoin;
        private readonly IMoonBitcoin _moonBitcoin;
        private readonly IBonusBitcoin _bonusBitcoin;
        private readonly IMoonDogecoin _moonDogecoin;
        private readonly IMoonLitecoin _moonLitecoin;
        private readonly IMoonDash _moonDash;
        private readonly IVLike _vLike;
        private readonly IVkTarget _vkTarget;
        private readonly IVkMyMarket _vkMyMarket;

        public StartController(IFreeBitcoin freeBitcoin, IMoonBitcoin moonBitcoin, IBonusBitcoin bonusBitcoin,
            IMoonDogecoin moonDogecoin, IMoonLitecoin moonLitecoin, IMoonDash moonDash, IVLike vLike, IVkTarget vkTarget,
            IVkMyMarket vkMyMarket)
        {
            _freeBitcoin = freeBitcoin;
            _moonBitcoin = moonBitcoin;
            _bonusBitcoin = bonusBitcoin;
            _moonDogecoin = moonDogecoin;
            _moonLitecoin = moonLitecoin;
            _moonDash = moonDash;
            _vLike = vLike;
            _vkTarget = vkTarget;
            _vkMyMarket = vkMyMarket;
        }

        public ActionResult Index() => View(WebService.GetAllData());

        [HttpGet]
        public PartialViewResult UpdateTimerCrane(Crane crane)
        {
            crane.ActivityTime -= TimeSpan.FromMinutes(5);
            if (crane.ActivityTime < TimeSpan.FromSeconds(1))
            {
                crane.ActivityTime = TimeSpan.FromSeconds(0);
            }

            WebService.UpdateCrane(crane);
            return PartialView("_Cranes", WebService.GetCranes());
        }

        [HttpGet]
        public PartialViewResult UpdateTimerService(InternetService service)
        {
            service.ActivityTime -= TimeSpan.FromMinutes(1);
            if (service.ActivityTime < TimeSpan.FromSeconds(1))
            {
                service.ActivityTime = TimeSpan.FromSeconds(0);
            }

            WebService.UpdateInternetService(service);
            return PartialView("_InternetService", WebService.GetInternetServices());
        }

        [HttpGet]
        public PartialViewResult UpdateStatusCrane(string url, Status statusCrane)
        {
            WebService.UpdateStatusCrane(url, statusCrane);
            return PartialView("_Cranes", WebService.GetCranes());
        }

        [HttpGet]
        public PartialViewResult UpdateStatusService(string url, Status statusService, bool isManualStart)
        {
            WebService.UpdateStatusService(url, statusService);

            var internetServices = WebService.GetInternetServices();
            if (isManualStart)
            {
                return PartialView("_ManualStart", internetServices);
            }

            return PartialView("_InternetService", internetServices);
        }

        [HttpGet]
        public PartialViewResult GoToCrane(Crane crane)
        {
            try
            {
                switch (crane.TypeCrane)
                {
                    case TypeCrane.FreeBitcoin:
                        crane = _freeBitcoin.Start(crane).Result;
                        break;
                    case TypeCrane.MoonBitcoin:
                        crane = _moonBitcoin.Start(crane).Result;
                        break;
                    case TypeCrane.BonusBitcoin:
                        crane = _bonusBitcoin.Start(crane).Result;
                        break;
                    case TypeCrane.MoonDogecoin:
                        crane = _moonDogecoin.Start(crane).Result;
                        break;
                    case TypeCrane.MoonLitecoin:
                        crane = _moonLitecoin.Start(crane).Result;
                        break;
                    case TypeCrane.MoonDash:
                        crane = _moonDash.Start(crane).Result;
                        break;
                }

                WebService.UpdateCrane(crane);
            }
            catch
            {
                if (crane != null)
                {
                    crane.StatusCrane = Status.NoWork;
                    WebService.UpdateCrane(crane);
                }
            }

            return PartialView("_Cranes", WebService.GetCranes());
        }

        [HttpGet]
        public PartialViewResult GoToInternetService(InternetService internetService)
        {
            switch (internetService.TypeService)
            {
                case TypeService.VLike:
                    internetService = _vLike.GoTo(internetService);
                    break;
            }

            WebService.UpdateInternetService(internetService);

            return PartialView("_InternetService", WebService.GetInternetServices());
        }

        [HttpGet]
        public void InternetServicesManualStart(string url, TypeService typeService)
        {
            switch (typeService)
            {
                case TypeService.VkTarget:
                    _vkTarget.GoTo(url);
                    break;
                case TypeService.VkMyMarket:
                    _vkMyMarket.GoTo(url);
                    break;
            }
        }

        [HttpGet]
        public PartialViewResult CloseBrowserManualStart(TypeService typeService)
        {
            switch (typeService)
            {
                case TypeService.VkTarget:
                    _vkTarget.Quit();
                    break;
            }

            return PartialView("_ManualStart", WebService.GetInternetServices());
        }

        [HttpGet]
        public PartialViewResult UpdateDataManualStartView()
        {
            return PartialView("_ManualStart", WebService.GetInternetServices());
        }

        [HttpPost]
        public ActionResult SaveAccounts(IFormFile fileAccounts) //ЕСТЬ TODO
        {
            if (fileAccounts == null)
            {
                return RedirectToAction("Index");
            }

            AccountManager accountManager = new AccountManager();  //TODO:Прокинуть через DI
            accountManager.SaveAccounts(fileAccounts);

            return RedirectToAction("Index");
        }
    }
}