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

        public StartController(IFreeBitcoin freeBitcoin, IRuCaptcha ruCaptcha)
        {
            _freeBitcoin = freeBitcoin;
            _ruCaptcha = ruCaptcha;
        }

        public ActionResult Index() => View(CraneService.GetCranes());

        [HttpGet]
        public PartialViewResult UpdateTimerCrane(Crane crane)
        {
            crane.ActivityTime = crane.ActivityTime - TimeSpan.FromMinutes(1);
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
            CheckingBid();

            switch (crane.TypeCrane)
            {
                case TypeCrane.FreeBitcoin:
                    crane = _freeBitcoin.GoTo(crane).Result;
                    CraneService.UpdateCrane(crane);
                    break;
            }

            return PartialView("_Cranes", CraneService.GetCranes());
        }

        protected void CheckingBid()
        {
            _ruCaptcha.GoTo();
        }
    }
}