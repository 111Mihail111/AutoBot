using AutoBot.Area.Interface;
using AutoBot.Area.Service;
using AutoBot.Enums;
using AutoBot.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AutoBot.Controllers
{
    public class StartController : Controller
    {
        private IFreeBitcoin _freeBitcoin;
        private IRuCaptcha _ruCaptcha;
        private IMoonBitcoin _moonBitcoin;
        private IBonusBitcoin _bonusBitcoin;

        public StartController(IFreeBitcoin freeBitcoin, IRuCaptcha ruCaptcha, IMoonBitcoin moonBitcoin, IBonusBitcoin bonusBitcoin)
        {
            _freeBitcoin = freeBitcoin;
            _ruCaptcha = ruCaptcha;
            _moonBitcoin = moonBitcoin;
            _bonusBitcoin = bonusBitcoin;
        }

        public ActionResult Index() => View(CraneService.GetCranes());

        [HttpGet]
        public PartialViewResult UpdateTimerCrane(Crane crane)
        {
            crane.ActivityTime -= TimeSpan.FromMinutes(1);
            if (crane.ActivityTime < TimeSpan.FromSeconds(1))
            {
                crane.ActivityTime = TimeSpan.FromSeconds(0);
            }

            CraneService.UpdateCrane(crane);
            return PartialView("_Cranes", CraneService.GetCranes());
        }

        [HttpGet]
        public PartialViewResult GoToCrane(Crane crane)
        {
            //CheckingBid();

            try
            {
                switch (crane.TypeCrane)
                {
                    case TypeCrane.FreeBitcoin:
                        crane = _freeBitcoin.GoTo(crane).Result;
                        break;
                    case TypeCrane.MoonBitcoin:
                        crane = _moonBitcoin.GoTo(crane).Result;
                        break;
                    case TypeCrane.BonusBitcoin:
                        crane = _bonusBitcoin.GoTo(crane).Result;
                        break;
                }
            }
            catch (Exception exeption)
            {
                crane.StatusCrane = Status.NoWork;
            }
            

            CraneService.UpdateCrane(crane);

            return PartialView("_Cranes", CraneService.GetCranes());
        }

        protected void CheckingBid()
        {
            _ruCaptcha.GoTo();
        }
    }
}