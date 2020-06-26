using AutoBot.Area.Interface;
using AutoBot.Area.Managers;
using AutoBot.Extentions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
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
            GoToUrl(url);

            if (!IsAuthorazition())
            {
                GetElementByXPath("/html/body/div[1]/div[1]/div/div/div/div[2]/div/ul/li[2]/a").Click();
                Thread.Sleep(2000);
                AuthorizationOnSite("email", "password", "btn_register", LOGIN, PASSWORD);
                GoToUrl(url);
            }
            
            ToChangeBet();
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

            var move = CreateActionToBrowser();
            var slider = GetElementByXPath("//*[@id='slider']/a");
            var direction = GetMyCurrentRate() < GetCurrentRate() ? true : false;

            MoveSlider(move, slider, direction);
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
        /// Движение слайдера
        /// </summary>
        /// <param name="actions">Действие в браузере</param>
        /// <param name="webElement">Веб-элемент, с которым производится действие</param>
        /// <param name="direction">Если true, элемент двигается влево, false - вправо</param>
        protected void MoveSlider(Actions actions, IWebElement webElement, bool direction)
        {
            for (int i = 1; i == 1;)
            {
                switch (direction)
                {
                    case true:
                        actions.DragAndDropToOffset(webElement, i, 0).Build().Perform();
                        break;
                    case false:
                        actions.DragAndDropToOffset(webElement, 0, i).Build().Perform();
                        break;
                }

                if (GetMyCurrentRate() >= GetCurrentRate())
                {
                    SetScrollPosition(0, 900);
                    GetElementByXPath("/html/body/div[1]/div[2]/div[2]/div/div[1]/div/form/div/div[30]/input").Click();
                    return;
                }
            }
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