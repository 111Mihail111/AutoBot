using AutoBot.Area.Managers;
using AutoBot.Models;
using System;
using System.Linq;

namespace AutoBot.Area.Cranes
{
    public class FreeBitcoin : BrowserManager
    {
        const string LOGIN = "polowinckin.mixail@yandex.ru";
        const string PASSWORD = "xHkKv78SvV2o7rSX";

        public void GoTo(Crane crane)
        {
            string urlCrane = crane.URL;

            GoToUrl(urlCrane);

            if (!CheckPage(urlCrane))
            {
                //Получение капчи и ее расшифровка
                AuthorizationOnCrane("signup_form_email", "signup_form_password", "signup_button", LOGIN, PASSWORD);
                GoToUrl(urlCrane);
            }

            SetPositionScrollTop(1000);

            if (CheckTimer())
            {
                crane.ActivityTime = TimeSpan.Parse(GetTimer());
                crane.BalanceOnCrane = BalanceCrane();
            }
        }

        public bool CheckPage(string url)
        {
            if (Browser.Url == url)
            {
                return true;
            }

            return false;
        }

        public bool CheckTimer()
        {
            if (GetElementsByXPath("//*[@id='time_remaining']").Any())
            {
                return true;
            }

            return false;
        }

        public string GetTimer()
        {
            var timer = GetElementByXPath("//*[@id='time_remaining']/span/").Text;
            timer = timer.Replace("Minutes", ":").Replace("Seconds", "");

            return TimeSpan.Parse(timer).ToString(@"hh\:mm\:ss");
        }

        public double BalanceCrane()
        {
            return Convert.ToDouble(GetElementByXPath("//*[@id='balance']").Text);
        }
    }
}
