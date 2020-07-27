using AutoBot.Area.Interface;
using AutoBot.Area.Managers;
using AutoBot.Extentions;
using AutoBot.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.Cranes
{
    public class BonusBitcoin : BrowserManager, IBonusBitcoin
    {
        private IRuCaptchaController _ruCaptchaController;
        const string LOGIN = "desiptikon.bot@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        const string PASSWORD = "123q_Q*W(*E&*R^*Z$*X!*C?*V";  //TODO: Настройки вынести отдельно на страницу
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\BonusBitcoin\\";
        private string _errorZeroBalance;

        public BonusBitcoin(IRuCaptchaController ruCaptchaController)
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

            if (CheckTimer())
            {
                return GetDetailsWithCrane(crane);
            }

            SetScrollPosition(1200);

            await DecipherCaptcha("g-recaptcha-response", urlCrane, "FaucetForm");
            GetElementByXPath("//*[@id='FaucetForm']/button[2]").Click();

            if (GetTabsCount() > 1)
            {
                CloseTab();
                SwitchToTab();
            }

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

            //TODO:Заменить на асинхронность
            var cookie = GetElementByXPath("/html/body/div[1]/div/a[1]");
            if (cookie != null && cookie.Displayed)
            {
                cookie.Click();
            }

            GetElementById("PageContent_SignInButton").Click();
            Thread.Sleep(1000);
            SetScrollPositionInWindow("SignInModal", 300);
            await DecipherCaptcha("g-recaptcha-response-1", urlCrane, "SignInForm");

            //TODO:Заменить на асинхронность
            var emailInput = GetElementByXPath("//*[@id='SignInEmailInput']");
            if (string.IsNullOrEmpty(emailInput.GetValue()))
            {
                emailInput.SendKeys(LOGIN);
            }

            //TODO:Заменить на асинхронность
            var passwordInput = GetElementByXPath("//*[@id='SignInPasswordInput']");
            if (string.IsNullOrEmpty(passwordInput.GetValue()))
            {
                passwordInput.SendKeys(PASSWORD);
            }

            ExecuteScript("document.querySelector('#SignInModal>div>div>div.modal-footer>button.btn.btn-primary').click()");

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
            CloseTab();

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
        protected async Task DecipherCaptcha(string textAreaId, string urlCrane, string elementWithToken)
        {
            string token = GetElementById(elementWithToken).GetDataFvAddonsRecaptcha2Sitekey();
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

            HiddenFieldVisible(textAreaId);
            GetElementByXPath(textAreaId).SendKeys(responseOnCaptcha);
            HiddenFieldInVisible(textAreaId);
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
