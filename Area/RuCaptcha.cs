using AutoBot.Area.Interface;
using AutoBot.Area.Managers;
using AutoBot.Enums;
using AutoBot.Extentions;
using System;
using System.Linq;
using System.Threading;

namespace AutoBot.Area
{
    public class RuCaptcha : BrowserManager, IRuCaptcha
    {
        protected const string LOGIN = "polowinckin.mixail@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        protected const string PASSWORD = "U394gbUGKUn3";  //TODO: Настройки вынести отдельно на страницу

        ///<inheritdoc/>
        public void GoTo()
        {
            string url = "https://rucaptcha.com/setting";

            Initialization("C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\RuCaptcha\\");
            GoToUrl(url);

            if (!IsAuthorazition())
            {
                GetElementByXPath("/html/body/div[1]/div[1]/div/div/div/div[2]/div/ul/li[2]/a").Click();
                Thread.Sleep(2000);
                AuthorizationOnSite(SearchMethod.Id, "email", "password", "btn_register", LOGIN, PASSWORD);
                GoToUrl(url);
            }

            ToChangeBet();
            CloseTab();
        }

        /// <summary>
        /// Поменять ставку
        /// </summary>
        protected void ToChangeBet()
        {
            if (GetCurrentRate() == GetMyCurrentRate())
            {
                return;
            }

            ExecuteScript($"document.getElementById('price').value = {GetCurrentRate()};");
            SetScrollPosition(900);
            GetElementByXPath("/html/body/div[1]/div[2]/div[2]/div/div[1]/div/form/div/div[30]/input").Click();
        }
        /// <summary>
        /// Получить текущую ставку за простые капчи
        /// </summary>
        /// <returns>Ставка</returns>
        protected int GetCurrentRate()
        {
            var webElement = GetElementByXPath("/html/body/div[1]/div[2]/div[2]/div/div[1]/div/form/div/div[12]/span").Text;
            return Convert.ToInt32(webElement);
        }
        /// <summary>
        /// Получить мою ставку за простые капчи
        /// </summary>
        /// <returns>Ставка</returns>
        protected int GetMyCurrentRate()
        {
            var webElement = GetElementById("price2").GetValue().Split(".").First();
            return Convert.ToInt32(webElement);
        }
        /// <summary>
        /// Проверка авторизации
        /// </summary>
        /// <param name="url">Url адрес страницы, доступной после авторизации</param>
        /// <returns>Если true - авторизация есть, иначе false</returns>
        protected bool IsAuthorazition()
        {
            return GetTitlePage() == "401 Unauthorized" ? false : true;
        }
    }
}