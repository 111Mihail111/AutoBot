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
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\MoonBitcoin\\";

        public async Task<Crane> GoTo(Crane crane)
        {
            string urlCrane = crane.URL;

            Initialization(BROWSER_PROFILE_CRANE);
            GoToUrl(urlCrane);
            await Authorization(urlCrane);

            if (IsButtonEnabled())
            {
                return GetDetailsWithCrane(crane);
            }

            OpenModalForCollectingCurrency(urlCrane);            
            await DecipherCaptcha(urlCrane, "ClaimForm");
            GetElementByXPath("//*[@id='ClaimModal']/div/div/div[3]/button[1]").Click();

            if (!IsCaptchaValid())
            {
                CloseTab();
                return await GoTo(crane);
            }

            return GetDetailsWithCrane(crane);
        }

        /// <summary>
        /// Открыть модальное окно для сбора криптовалюты
        /// </summary>
        /// <param name="urlCrane">URL-адрес крана</param>
        private void OpenModalForCollectingCurrency(string urlCrane)
        {
            int countTabs = 2;
            while (countTabs == 2)
            {
                RemovePromotionalBlock();
                GetElementByXPath("//*[@id='Faucet']/div[2]/button").Click();

                if (GetTabsCount() < countTabs)
                {
                    break;
                }

                CloseTab();
                SwitchToTab();
                GoToUrl(urlCrane);
            }
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

            SetScrollPosition(300);
            GetElementByXPath("//*[@id='PageContent_UnauthorisedButtons']/button").Click();
            Thread.Sleep(1200);

            var signInEmailInput = GetElementById("SignInEmailInput");
            if (string.IsNullOrEmpty(signInEmailInput.GetValue()))
            {
                signInEmailInput.SendKeys(LOGIN);
            }

            await DecipherCaptcha(urlCrane, "SignInForm");

            string signInClick = "document.querySelector('#SignInModal>div>div>div.modal-footer>button').click();";
            ExecuteScript(signInClick);

            int tabs = GetTabsCount();
            while (tabs > 1)
            {
                SwitchToLastTab();
                CloseTab();
                SwitchToLastTab();
                ExecuteScript(signInClick);
                tabs = GetTabsCount();
            }

            if (!IsCaptchaValid())
            {
                RefreshPage();
                await Authorization(urlCrane);
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
            crane.ActivityTime = TimeSpan.FromMinutes(10);

            CloseTab();

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
        protected async Task DecipherCaptcha(string urlCrane, string elementId)
        {
            string token = GetElementById(elementId).GetDataFvAddonsRecaptcha2Sitekey();
            string responseOnCaptcha = ERROR_CAPTCHA_UNSOLVABLE;

            while (responseOnCaptcha == ERROR_CAPTCHA_UNSOLVABLE || responseOnCaptcha == ERROR_BAD_DUPLICATES)
            {
                responseOnCaptcha = await _ruCaptchaController.SendCaptcha_v2(token, urlCrane);
            }

            HiddenFieldVisible("g-recaptcha-response");
            GetElementByXPath("//*[@id='g-recaptcha-response']").SendKeys(responseOnCaptcha);
            HiddenFieldInVisible("g-recaptcha-response");
        }

        protected bool IsCaptchaValid()
        {
            if (!GetElementById("MessageModal").Displayed)
            {
                _ruCaptchaController.SendReportOnCaptcha(_ruCaptchaController.GetKeyCaptcha(), "reportgood");
                return true;
            }

            string errorCaptcha = GetElementByXPath("//*[@id='MessageModal']/div/div/div[2]").GetInnerText();
            if (errorCaptcha.Contains("The captcha is not valid"))
            {
                _ruCaptchaController.SendReportOnCaptcha(_ruCaptchaController.GetKeyCaptcha(), "reportbad");
            }

            return false;
        }

    }
}
