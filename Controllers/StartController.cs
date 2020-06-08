using AutoBot.Area.Cranes;
using AutoBot.Area.Managers;
using AutoBot.Enums;
using AutoBot.Models;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

namespace AutoBot.Controllers
{
    public class StartController : Controller
    {

        public ActionResult Index() => View(InitCranes());

        protected List<Crane> InitCranes()
        {
            
            return new List<Crane> 
            { 
                new Crane { URL = "https://freebitco.in/", ActivityTime = TimeSpan.FromHours(0), 
                    BalanceOnCrane = 0, StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.Bitcoin, TypeCrane = TypeCrane.FreeBitcoin },
                //new Crane { URL = "https://freebitco.in/", ActivityTime = TimeSpan.(0),
                //    MyBalanceOnCrane = 0, StatusCrane = Status.Work, TypeCurrencies = TypeCurrencies.Bitcoin },
            };
        }
        
        [HttpGet]
        public JsonResult UpdateTimerCrane(string timer)
        {
            timer = (TimeSpan.Parse(timer) - new TimeSpan(1)).ToString(@"hh\:mm\:ss");
            return Json(timer);
        }

        [HttpGet]
        public ActionResult GoToCrane(Crane crane)
        {
            GoTo(crane);
            return null;
        }

        public void GoTo(Crane crane)
        {
            switch (crane.TypeCrane)
            {
                case TypeCrane.FreeBitcoin:
                    FreeBitcoin freeBitcoin = new FreeBitcoin();
                    freeBitcoin.GoTo(crane);
                    break;
            }
        }
    }
}