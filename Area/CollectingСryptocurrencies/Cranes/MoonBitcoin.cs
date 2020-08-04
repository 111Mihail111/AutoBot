using AutoBot.Area.CollectingСryptocurrencies.Interface;
using AutoBot.Area.Managers;
using AutoBot.Extentions;
using AutoBot.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.CollectingСryptocurrencies.Cranes
{
    public class MoonBitcoin : BrowserManager, IMoonBitcoin
    {
        private IRuCaptchaController _ruCaptchaController;
        const string LOGIN = "polowinckin.mixail@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\MoonBitcoin\\";
        private string _errorZeroBalance;

        public MoonBitcoin(IRuCaptchaController ruCaptchaController)
        {
            _ruCaptchaController = ruCaptchaController;
        }
        
        ///<inheritdoc/>
        public async Task<Crane> Start(Crane crane)
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
                QuitBrowser();
                return await Start(crane);
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
                Thread.Sleep(1000);

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
            Thread.Sleep(600);

            var signInEmailInput = GetElementById("SignInEmailInput");
            if (string.IsNullOrEmpty(signInEmailInput.GetValue()))
            {
                signInEmailInput.SendKeys(LOGIN);
            }

            await DecipherCaptcha(urlCrane, "SignInForm");

            string signInClick = "document.querySelector('#SignInModal>div>div>div.modal-footer>button').click();";
            ExecuteScript(signInClick);

            while (GetTabsCount() > 1)
            {
                SwitchToLastTab();
                CloseTab();
                SwitchToLastTab();
                ExecuteScript(signInClick);
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
        protected async Task DecipherCaptcha(string urlCrane, string elementId)
        {
            string token = GetElementById(elementId).GetDataFvAddonsRecaptcha2Sitekey();
            string responseOnCaptcha = ERROR_CAPTCHA_UNSOLVABLE;

            while (responseOnCaptcha == ERROR_CAPTCHA_UNSOLVABLE || responseOnCaptcha == ERROR_BAD_DUPLICATES)
            {
                responseOnCaptcha = await _ruCaptchaController.SendRecaptcha_v2(token, urlCrane);
            }

            if (responseOnCaptcha == ERROR_ZERO_BALANCE)
            {
                _errorZeroBalance = responseOnCaptcha;
                return;
            }

            HiddenFieldVisible("g-recaptcha-response");
            GetElementByXPath("//*[@id='g-recaptcha-response']").SendKeys(responseOnCaptcha);
            HiddenFieldInVisible("g-recaptcha-response");
        }
        /// <summary>
        /// Валидна ли капча
        /// </summary>
        /// <returns>True если валидна, иначе false</returns>
        protected bool IsCaptchaValid()
        {
            if (!GetElementById("MessageModal").Displayed)
            {
                _ruCaptchaController.SendReport(_ruCaptchaController.GetCaptchaQueryId(), "reportgood");
                return true;
            }

            string errorCaptcha = GetElementByXPath("//*[@id='MessageModal']/div/div/div[2]").GetInnerText();
            if (errorCaptcha.Contains("The captcha is not valid"))
            {
                _ruCaptchaController.SendReport(_ruCaptchaController.GetCaptchaQueryId(), "reportbad");
            }

            return false;
        }
    }
}
