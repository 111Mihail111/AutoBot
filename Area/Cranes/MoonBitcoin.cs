using AutoBot.Area.API;
using AutoBot.Area.Interface;
using AutoBot.Area.Managers;
using AutoBot.Extentions;
using AutoBot.Models;
using OpenQA.Selenium.Chrome;
using System;
using System.Security.Policy;
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

            Initialization(new ChromeOptions());
            GoToUrl(urlCrane);
            await Authorization(urlCrane);
            RemovePromotionalBlock();

            if (IsButtonEnabled())
            {
                return GetDetailsWithCrane(crane);
            }
            
            GetElementByXPath("//*[@id='Faucet']/div[2]/button").Click();

            string token = GetElementById("ClaimForm").GetDataFvAddonsRecaptcha2Sitekey();
            string response = await DecipherCaptcha(token, urlCrane);

            if (response == ERROR_CAPTCHA_UNSOLVABLE)
            {
                return await GoTo(crane);
            }
                        
            GetElementByXPath("//*[@id='ClaimModal']/div/div/div[3]/button[1]").Click();
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
            if (GetUrlPage() == urlCrane)
            {
                return;
            }

            string response = ERROR_CAPTCHA_UNSOLVABLE;
            while (response == ERROR_CAPTCHA_UNSOLVABLE)
            {
                SetScrollPosition(300);
                GetElementByXPath("//*[@id='PageContent_UnauthorisedButtons']/button").Click(); 
                Thread.Sleep(1200);
                GetElementById("SignInEmailInput").SendKeys(LOGIN);

                string token = GetElementById("SignInForm").GetDataFvAddonsRecaptcha2Sitekey();
                response = await DecipherCaptcha(token, urlCrane);

                if (response == ERROR_CAPTCHA_UNSOLVABLE)
                {
                    RefreshPage();
                    continue;
                }

                ExecuteScript("document.querySelector('#SignInModal>div>div>div.modal-footer>button').click();");
            }

            int tabs = GetTabsCount();
            while (tabs != 1)
            {
                SwitchToLastTab();
                CloseTab();
                SwitchToTab();
                ExecuteScript("document.querySelector('#SignInModal>div>div>div.modal-footer>button').click();");
                tabs = GetTabsCount();
            }
        }
        /// <summary>
        /// Скрытое поле видимое
        /// </summary>
        /// <param name="elementId">Путь к элементу</param>
        protected void HiddenFieldVisible(string elementId)
        {
            ExecuteScript($"var element = document.getElementById('{elementId}');" +
                "element.style.position = 'absolute';" +
                "element.style.display = 'inline';");
        }
        /// <summary>
        /// Скрытое поле невидимое
        /// </summary>
        /// <param name="elementId">Путь к элементу</param>
        protected void HiddenFieldInVisible(string elementId)
        {
            ExecuteScript($"var element = document.getElementById('{elementId}');" +
                "element.style.position = '';" +
                "element.style.display = 'none';");
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

            QuitBrowser();

            return crane;
        }
        /// <summary>
        /// Заблокированна ли кнопка (снятия валюты)
        /// </summary>
        /// <returns></returns>
        protected bool IsButtonEnabled()
        {
            return GetElementByXPath("//*[@id='Faucet']/div[2]/button").Enabled == false;
        }
        /// <summary>
        /// Расшифровать капчу
        /// </summary>
        /// <param name="token">Токен рекапчи</param>
        /// <param name="urlCrane">Url-адрес крана</param>
        /// <returns>Расшифрованный токен капчи или ошибку от сервиса RuCaptcha</returns>
        protected async Task<string> DecipherCaptcha(string token, string urlCrane)
        {
            string responseOnCaptcha = await _ruCaptchaController.SendCaptcha_v2(token, urlCrane);
            
            HiddenFieldVisible("g-recaptcha-response");
            GetElementByXPath("//*[@id='g-recaptcha-response']").SendKeys(responseOnCaptcha);
            HiddenFieldInVisible("g-recaptcha-response");

            return responseOnCaptcha;
        }
    }
}
