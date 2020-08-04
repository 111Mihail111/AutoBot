using AutoBot.Area.CollectingСryptocurrencies.Interface;
using AutoBot.Area.Managers;
using AutoBot.Enums;
using AutoBot.Extentions;
using System;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.CollectingСryptocurrencies
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
            Authorazition(url);

            ToChangeBet();
            CloseTab();
        }

        protected void Authorazition(string url)
        {
            if (!IsAuthorazition())
            {
                return;
            }

            //TODO: В будущем может понадобиться расшифровка капчи

            GetElementByXPath("/html/body/div[1]/div[1]/div/div/div/div[2]/div/ul/li[2]/a").Click();
            Thread.Sleep(1000);
            AuthorizationOnSite(SearchMethod.Id, "email", "password", "btn_register", LOGIN, PASSWORD);
            GoToUrl(url);
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

            //TODO:Протестить
            //SetScrollPosition(900);

            string buttonSave = "/html/body/div[1]/div[3]/div/div/div/div/div[1]/div/form/div/div[13]/input";
            GetElementByXPath(buttonSave).Click();
        }
        /// <summary>
        /// Получить текущую ставку за простые капчи
        /// </summary>
        /// <returns>Ставка</returns>
        protected int GetCurrentRate()
        {
            string xPath = "/html/body/div[1]/div[3]/div/div/div/div/div[1]/div/form/div/div[6]/div[2]";
            string currentRate = GetElementByXPath(xPath).GetInnerText().Replace(" руб / за каждую 1000 капч", string.Empty);

            return Convert.ToInt32(currentRate);
        }
        /// <summary>
        /// Получить мою ставку за простые капчи
        /// </summary>
        /// <returns>Ставка</returns>
        protected int GetMyCurrentRate()
        {
            string myCurrent = GetElementById("price").GetValue().Split(".").First();

            return Convert.ToInt32(myCurrent);
        }

        /// <summary>
        /// Проверка авторизации
        /// </summary>
        /// <param name="url">Url адрес страницы, доступной после авторизации</param>
        /// <returns>Если true - авторизация есть, иначе false</returns>
        protected bool IsAuthorazition()
        {
            return GetTitlePage() == "401 Unauthorized";
        }
    }
}