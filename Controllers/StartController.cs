using AutoBot.Area.Interface;
using AutoBot.Area.Service;
using AutoBot.Enums;
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

        public StartController(IFreeBitcoin freeBitcoin, IMoonBitcoin moonBitcoin, IBonusBitcoin bonusBitcoin, 
            IMoonDogecoin moonDogecoin, IMoonLitecoin moonLitecoin)
        {
            _freeBitcoin = freeBitcoin;
            _moonBitcoin = moonBitcoin;
            _bonusBitcoin = bonusBitcoin;
            _moonDogecoin = moonDogecoin;
            _moonLitecoin = moonLitecoin;
        }

        public ActionResult Index() => View(CraneService.GetCranes());


        [HttpGet]
        public PartialViewResult UpdateTimerCrane(Crane crane)
        {
            crane.ActivityTime -= TimeSpan.FromMinutes(10);
            if (crane.ActivityTime < TimeSpan.FromSeconds(1))
            {
                crane.ActivityTime = TimeSpan.FromSeconds(0);
            }

            CraneService.UpdateCrane(crane);
            return PartialView("_Cranes", CraneService.GetCranes());
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
                }

                CraneService.UpdateCrane(crane);
            }
            catch (Exception exeption)
            {
                if (crane != null)
                {
                    crane.StatusCrane = Status.NoWork;
                    CraneService.UpdateCrane(crane);
                }
            }

            return PartialView("_Cranes", CraneService.GetCranes());
        }

    }
}