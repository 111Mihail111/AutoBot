using AutoBot.Area.API;
using AutoBot.Area.Interface;
using AutoBot.Area.Managers;
using AutoBot.Enums;
using AutoBot.Extentions;
using AutoBot.Models;
using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.Cranes
{
    public class BonusBitcoin : BrowserManager, IBonusBitcoin
    {
        private RuCaptchaController _ruCaptchaController = new RuCaptchaController(); //TODO: Обернуть интерфейсом и прокинуть через DI
        const string LOGIN = "desiptikon.bot@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        const string PASSWORD = "123q_Q*W(*E&*R^*Z$*X!*C?*V";  //TODO: Настройки вынести отдельно на страницу

        public async Task<Crane> GoTo(Crane crane)
        {
            string urlCrane = crane.URL;

            GoToUrl(urlCrane);
            await Authorization(urlCrane);

            if (CheckTimer())
            {
                return GetDetailsWithCrane(crane);
            }

            SetScrollPosition(1200);

            string token = GetElementById("FaucetForm").GetDataFvAddonsRecaptcha2Sitekey();
            string response = await DecipherCaptcha("g-recaptcha-response", token, urlCrane);

            if (response == ERROR_CAPTCHA_UNSOLVABLE)
            {
                return await GoTo(crane);
            }

            GetElementByXPath("//*[@id='FaucetForm']/button[2]").Click();
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
            if (urlCrane == GetUrlPage())
            {
                return;
            }

            GetElementByXPath("/html/body/div[1]/div/a[1]").Click();

            string response = ERROR_CAPTCHA_UNSOLVABLE;
            while (response == ERROR_CAPTCHA_UNSOLVABLE)
            {
                GetElementById("PageContent_SignInButton").Click();
                Thread.Sleep(1000);
                SetScrollPositionInWindow("SignInModal", 300);

                string token = GetElementById("SignInForm").GetDataFvAddonsRecaptcha2Sitekey();
                response = await DecipherCaptcha("g-recaptcha-response-1", token, urlCrane);

                if (response == ERROR_CAPTCHA_UNSOLVABLE)
                {
                    RefreshPage();
                    continue;
                }

                GetElementByXPath("//*[@id='SignInEmailInput']").SendKeys(LOGIN);
                GetElementByXPath("//*[@id='SignInPasswordInput']").SendKeys(PASSWORD);
                ExecuteScript("document.querySelector('#SignInModal>div>div>div.modal-footer>button.btn.btn-primary').click()");
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
        /// Проверка авторизации
        /// </summary>
        /// <param name="urlCrane">URL адрес крана</param>
        /// <returns></returns>
        protected bool CheckAuthorization(string urlCrane)
        {
            return urlCrane != GetUrlPage();
        }
        /// <summary>
        /// Получить баланс с крана
        /// </summary>
        /// <returns>Баланс</returns>
        protected string GetBalanceOnCrane()
        {
            return GetElementById("BalanceInput").GetValue();
        }
        /// <summary>
        /// Получить таймер
        /// </summary>
        /// <returns>Время таймера</returns>
        protected TimeSpan GetTimer()
        {
            var timer = "00:";
            timer += GetElementByXPath("//*[@id='FaucetForm']/button[2]").GetInnerText();

            return TimeSpan.Parse(timer.Replace("Claim again in ", string.Empty));
        }
        /// <summary>
        /// Получить детали с крана.
        /// </summary>
        /// <param name="crane">Модель крана</param>
        /// <returns>Обновленная модель крана</returns>
        protected Crane GetDetailsWithCrane(Crane crane)
        {
            crane.ActivityTime = GetTimer();
            crane.BalanceOnCrane = GetBalanceOnCrane();

            return crane;
        }
        /// <summary>
        /// Проверка таймера
        /// </summary>
        /// <returns>True элемент не доступен, иначе false</returns>
        public bool CheckTimer()
        {
            return GetElementByXPath("//*[@id='FaucetForm']/button[2]").Enabled == false;
        }
        /// <summary>
        /// Расшифровать капчу
        /// </summary>
        /// <param name="token">Токен рекапчи</param>
        /// <param name="urlCrane">Url-адрес крана</param>
        /// <returns>Расшифрованный токен капчи или ошибку от сервиса RuCaptcha</returns>
        protected async Task<string> DecipherCaptcha(string elementId, string token, string urlCrane)
        {
            string responseOnCaptcha = await _ruCaptchaController.SendCaptcha_v2(token, urlCrane);

            HiddenFieldVisible(elementId);
            GetElementByXPath(elementId).SendKeys(responseOnCaptcha);
            HiddenFieldInVisible(elementId);
            
            return responseOnCaptcha;
        }
    }
}
