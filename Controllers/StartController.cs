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
        private IMoonBitcoin _moonBitcoin;
        private IBonusBitcoin _bonusBitcoin;

        public StartController(IFreeBitcoin freeBitcoin, IMoonBitcoin moonBitcoin, IBonusBitcoin bonusBitcoin)
        {
            _freeBitcoin = freeBitcoin;
            _moonBitcoin = moonBitcoin;
            _bonusBitcoin = bonusBitcoin;
        }

        public ActionResult Index() => View(CraneService.GetCranes());


        [HttpGet]
        public PartialViewResult UpdateTimerCrane(Crane crane)
        {
            crane.ActivityTime -= TimeSpan.FromMinutes(7);
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