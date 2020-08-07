using AutoBot.Area.CollectingСryptocurrencies.Interface;
using AutoBot.Area.Enums;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AutoBot.Controllers
{
    public class StartController : Controller
    {
        private IFreeBitcoin _freeBitcoin;
        private IMoonBitcoin _moonBitcoin;
        private IBonusBitcoin _bonusBitcoin;
        private IMoonDogecoin _moonDogecoin;
        private IMoonLitecoin _moonLitecoin;
        private IMoonDash _moonDash;
        private IV_Like _v_Like;

        public StartController(IFreeBitcoin freeBitcoin, IMoonBitcoin moonBitcoin, IBonusBitcoin bonusBitcoin, 
            IMoonDogecoin moonDogecoin, IMoonLitecoin moonLitecoin, IMoonDash moonDash, IV_Like v_Like)
        {
            _freeBitcoin = freeBitcoin;
            _moonBitcoin = moonBitcoin;
            _bonusBitcoin = bonusBitcoin;
            _moonDogecoin = moonDogecoin;
            _moonLitecoin = moonLitecoin;
            _moonDash = moonDash;
            _v_Like = v_Like;
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
        public PartialViewResult UpdateStatusCrane(string url, Status statusCrane)
        {
            WebService.UpdateStatusCrane(url, statusCrane);
            return PartialView("_Cranes", WebService.GetCranes());
        }

        [HttpGet]
        public async Task<PartialViewResult> GoToCrane(Crane crane)
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
            catch (Exception exeption)
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
                case TypeService.V_Like:
                    internetService = _v_Like.GoTo(internetService);
                    break;
            }

            WebService.UpdateInternetService(internetService);

            return PartialView("_InternetService", WebService.GetInternetServices());
        }
    }
}