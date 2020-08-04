using AutoBot.Area.API;
using AutoBot.Area.CollectingСryptocurrencies.Interface;
using AutoBot.Area.Managers;
using AutoBot.Extentions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.CollectingСryptocurrencies
{
    public class RuCaptcha : BrowserManager, IRuCaptcha
    {
        private RuCaptchaController _ruCaptchaController = new RuCaptchaController(); //TODO:Сделать через ко
        protected const string LOGIN = "polowinckin.mixail@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        protected const string PASSWORD = "U394gbUGKUn3";  //TODO: Настройки вынести отдельно на страницу
        const string BROWSER_PROFILE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\RuCaptcha\\";
        private string _errorZeroBalance;


        ///<inheritdoc/>
        public async Task GoTo()
        {
            string url = "https://rucaptcha.com/setting";

            Initialization(BROWSER_PROFILE);
            GoToUrl(url);
            await Authorazition(url);

            ToChangeBet();
            QuitBrowser();
        }

        protected async Task Authorazition(string url)
        {
            if (!IsAuthorazition())
            {
                return;
            }

            GetElementByXPath("/html/body/div[1]/div[1]/div/div/div/div[2]/div/ul/li[2]/a").Click();
            Thread.Sleep(1000);
            
            InsertLoginAndPassword();
            
            var switcher = GetElementByXPath("//*[@id='switch_div']/label[2]/div");
            if (switcher != null && switcher.Displayed)
            {
                switcher.Click();
            }
            else
            {
                GetElementById("btn_register").Click();
                GoToUrl(url);
                return;
            }
            
            await DecipherCaptcha(url, "//*[@id='re_captcha']/div/div");
            GetElementById("btn_register").Click();

            if (!IsCaptchaValid())
            {
                RefreshPage();
                await Authorazition(url);
                return;
            }

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
        /// <summary>
        /// Расшифровать капчу
        /// </summary>
        /// <param name="token">Токен рекапчи</param>
        /// <param name="urlCrane">Url-адрес крана</param>
        /// <returns>Расшифрованный токен капчи или ошибку от сервиса RuCaptcha</returns>
        protected async Task DecipherCaptcha(string urlCrane, string elementId)
        {
            string token = GetElementByXPath(elementId).GetDataSitekey();
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
        /// Валидна ли капча
        /// </summary>
        /// <returns>True если валидна, иначе false</returns>
        protected bool IsCaptchaValid()
        {
            string xPath = "/html/body/div[1]/div/div/div/div/div/div/div[2]";
            if (!GetElementByXPath(xPath).Displayed)
            {
                _ruCaptchaController.SendReport(_ruCaptchaController.GetCaptchaQueryId(), "reportgood");
                return true;
            }

            string errorCaptcha = GetElementByXPath(xPath).GetInnerText();
            if (errorCaptcha.Contains("Капча введена неверно"))
            {
                _ruCaptchaController.SendReport(_ruCaptchaController.GetCaptchaQueryId(), "reportbad");
            }

            return false;
        }
        /// <summary>
        /// Ввести логин и пароль в поля
        /// </summary>
        protected async void InsertLoginAndPassword()
        {
            var emailInput = await Task.Run(() => GetAsyncElementById("password"));
            if (emailInput != null && string.IsNullOrEmpty(emailInput.GetValue()))
            {
                emailInput.SendKeys(PASSWORD);
            }

            var passwordInput = Task.Run(() => GetAsyncElementById("email")).Result;
            if (passwordInput != null && string.IsNullOrEmpty(passwordInput.GetValue()))
            {
                passwordInput.SendKeys(LOGIN);
            }
        }
    }
}