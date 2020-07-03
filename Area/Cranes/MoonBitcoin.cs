using AutoBot.Area.API;
using AutoBot.Area.Interface;
using AutoBot.Area.Managers;
using AutoBot.Extentions;
using AutoBot.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.Cranes
{
    public class MoonBitcoin : BrowserManager, IMoonBitcoin
    {
        private RuCaptchaController _ruCaptchaController = new RuCaptchaController(); //TODO: Обернуть интерфейсом и прокинуть через DI
        const string LOGIN = "polowinckin.mixail@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        

        public async Task<Crane> GoTo(Crane crane)
        {
            string urlCrane = crane.URL;

            GoToUrl(urlCrane);

            if (CheckAuthorization(urlCrane))
            {
                await Authorization(urlCrane);
            }

            RemovePromotionalBlock();

            GetElementByXPath("//*[@id='Faucet']/div[2]/button").Click();
            string token = GetElementById("ClaimForm").GetDataFvAddonsRecaptcha2Sitekey();
            string responseOnCaptcha = await _ruCaptchaController.SendCaptcha_v2(token, urlCrane);

            HiddenFieldVisible("g-recaptcha-response");
            GetElementByXPath("//*[@id='g-recaptcha-response']").SendKeys(responseOnCaptcha);
            HiddenFieldInVisible("g-recaptcha-response");
            GetElementById("ClaimModal").Click();
            GetElementByXPath("//*[@id='FaucetClaimModal']/div/div/div[3]/button").Click();

            return GetDetailsWithCrane(crane);
        }


        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="urlCrane">URL крана</param>
        /// <returns>Асинхронная задача</returns>
        protected async Task Authorization(string urlCrane)
        {
            SetScrollPosition(300);
            GetElementByXPath("//*[@id='PageContent_UnauthorisedButtons']/button").Click();
            Thread.Sleep(2000);
            GetElementById("SignInEmailInput").SendKeys(LOGIN);

            string token = GetElementById("SignInForm").GetDataFvAddonsRecaptcha2Sitekey();
            string responseOnCaptcha = await _ruCaptchaController.SendCaptcha_v2(token, urlCrane);

            HiddenFieldVisible("g-recaptcha-response");
            GetElementByXPath("//*[@id='g-recaptcha-response']").SendKeys(responseOnCaptcha);
            HiddenFieldInVisible("g-recaptcha-response");
            GetElementByXPath("//*[@id='SignInModal']/div/div/div[3]/button").Click();
            //TODO: Нужна проверка на то, открыта ли новая вкладка после нажатия на кнопку.
        }
        /// <summary>
        /// Скрытое поле видимое
        /// </summary>
        /// <param name="xPath">Путь к элементу</param>
        protected void HiddenFieldVisible(string xPath)
        {
            ExecuteScript($"var element = document.getElementById('{xPath}');" +
                "element.style.position = 'absolute';" +
                "element.style.display = 'inline';");
        }
        /// <summary>
        /// Скрытое поле невидимое
        /// </summary>
        /// <param name="xPath">Путь к элементу</param>
        protected void HiddenFieldInVisible(string xPath)
        {
            ExecuteScript($"var element = document.getElementById('{xPath}');" +
                "element.style.position = '';" +
                "element.style.display = 'none';");
        }
        /// <summary>
        /// Проверка авторизации
        /// </summary>
        /// <param name="urlCrane">URL адрес крана</param>
        /// <returns></returns>
        protected bool CheckAuthorization(string urlCrane)
        {
            return urlCrane != GetUrlPage();
        }
        /// <summary>
        /// Удалить рекламный блок
        /// </summary>
        protected void RemovePromotionalBlock()
        {
            ExecuteScript("document.getElementById('slideIn').remove();");
        }
        /// <summary>
        /// Получить детали с крана.
        /// </summary>
        /// <param name="crane">Модель крана</param>
        /// <returns>Обновленная модель крана</returns>
        protected Crane GetDetailsWithCrane(Crane crane)
        {
            crane.BalanceOnCrane = GetElementByXPath("//*[@id='Navigation']/div/span/a").Text;
            crane.ActivityTime = TimeSpan.FromHours(1);

            return crane;
        }
    }
}
