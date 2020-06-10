using AutoBot.Area.Cranes;
using AutoBot.Area.Managers;
using AutoBot.Area.Service;
using AutoBot.Enums;
using AutoBot.Models;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoBot.Controllers
{
    public class StartController : Controller
    {       
        public ActionResult Index() => View(CraneService.GetCranes());
            
        [HttpGet]
        public PartialViewResult UpdateTimerCrane(Crane crane)
        {
            crane.ActivityTime = crane.ActivityTime - TimeSpan.FromMinutes(1);
            CraneService.UpdateCrane(crane);

            return PartialView("_Cranes", CraneService.GetCranes());
        }

        [HttpGet]
        public PartialViewResult GoToCrane(Crane crane)
        {
            switch (crane.TypeCrane)
            {
                case TypeCrane.FreeBitcoin:
                    FreeBitcoin freeBitcoin = new FreeBitcoin();
                    freeBitcoin.GoTo(crane);
                    //После выполнения кран возвращает модель. Отдаем ее на обновление
                    //CraneService.UpdateCrane(crane);
                    break;
            }

            return PartialView("_Cranes", CraneService.GetCranes());
        }
    }
}