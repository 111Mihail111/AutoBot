using AutoBot.Area.CollectingСryptocurrencies.Interface;
using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.Services;
using AutoBot.Extentions;
using AutoBot.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.CollectingСryptocurrencies.Cranes
{
    public class BonusBitcoin : BrowserManager, IBonusBitcoin
    {
        private IRuCaptchaController _ruCaptchaController;
        private string _login;
        private string _password;
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\BonusBitcoin\\";
        private string _errorZeroBalance;

        public BonusBitcoin(IRuCaptchaController ruCaptchaController)
        {
            _ruCaptchaController = ruCaptchaController;
        }

        protected void Init()
        {
            var account = AccountService.GetAccountsByType(TypeCrane.BonusBitcoin).First();
            _login = account.Login;
            _password = account.Password;

            Initialization(BROWSER_PROFILE_CRANE);
        }

        ///<inheritdoc/>
        public async Task<Crane> Start(Crane crane)
        {
            Init();

            string urlCrane = crane.URL;
            GoToUrl(urlCrane);
            await Authorization(urlCrane);

            if (CheckTimer())
            {
                return GetDetailsWithCrane(crane);
            }

            await DecipherCaptcha("g-recaptcha-response", urlCrane, "FaucetForm");
            GetElementByXPath("//*[@id='FaucetForm']/button[2]").Click();

            if (!IsCaptchaValid())
            {
                QuitBrowser();
                return await Start(crane);
            }

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

            ConsentToCookies();
            GetElementById("PageContent_SignInButton").Click();
            Thread.Sleep(1000);

            InsertLoginAndPassword();
            SetScrollPositionInWindow("SignInModal", 300);
            await DecipherCaptcha("g-recaptcha-response-1", urlCrane, "SignInForm");
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
            crane.StatusCrane = Status.Work;

            QuitBrowser();

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
        /// <summary>
        /// Согласие на куки
        /// </summary>
        protected async void ConsentToCookies()
        {
            var cookie = await GetAsyncElementByXPath("/html/body/div[1]/div/a[1]");
            if (cookie != null && cookie.Displayed)
            {
                cookie.Click();
            }
        }
        /// <summary>
        /// Ввести логин и пароль в поля
        /// </summary>
        protected async void InsertLoginAndPassword()
        {
            var emailInput = await Task.Run(() => GetAsyncElementByXPath("//*[@id='SignInEmailInput']"));
            if (emailInput != null && string.IsNullOrEmpty(emailInput.GetValue()))
            {
                emailInput.SendKeys(_login);
            }

            var passwordInput = await Task.Run(() => GetAsyncElementByXPath("//*[@id='SignInPasswordInput']"));
            if (passwordInput != null && string.IsNullOrEmpty(passwordInput.GetValue()))
            {
                passwordInput.SendKeys(_password);
            }
        }
    }
}
