using AutoBot.Area.Interface;
using AutoBot.Area.Managers;
using AutoBot.Enums;
using AutoBot.Extentions;
using AutoBot.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.Cranes
{
    public class FreeBitcoin : BrowserManager, IFreeBitcoin
    {
        private IRuCaptchaController _ruCaptchaController;
        const string LOGIN = "polowinckin.mixail@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        const string PASSWORD = "xHkKv78SvV2o7rSX";  //TODO: Настройки вынести отдельно на страницу
        private string _errorZeroBalance;

        public FreeBitcoin(IRuCaptchaController ruCaptchaController)
        {
            _ruCaptchaController = ruCaptchaController;
        }

        ///<inheritdoc/>
        public async Task<Crane> Start(Crane crane)
        {
            string urlCrane = crane.URL;

            Initialization("C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\FreeBitcoin\\");
            GoToUrl(urlCrane);
            Thread.Sleep(1000);
            await AuthorizationOnCrane(urlCrane);
            
            if (IsTimerExist())
            {
                return GetDetailsWithCrane(crane);
            }

            SetScrollPosition(1000);
            await DecipherCaptcha(urlCrane, "//*[@id='free_play_recaptcha']/form/div");
            GetElementById("free_play_form_button").Click();

            if (!IsCaptchaValid())
            {
                CloseTab();
                return await Start(crane);
            }

            GetElementByXPath("//*[@id='myModal22']/a").Click();

            return GetDetailsWithCrane(crane);
        }
        /// <summary>
        /// Авторизоваться на кране
        /// </summary>
        /// <param name="urlCrane">URL-адрес крана</param>
        /// <returns>Асинхронная задача</returns>
        protected async Task AuthorizationOnCrane(string urlCrane)
        {
            if (GetUrlPage() == urlCrane)
            {
                return;
            }

            var noThanks = GetElementByXPath("//*[@id='push_notification_modal']/div[1]/div[2]/div/div[1]");
            if (noThanks.Displayed)
            {
                noThanks.Click();
            }

            var cookie = GetElementByXPath("/html/body/div[1]/div/a[1]");
            if (cookie.Displayed)
            {
                cookie.Click();
            }

            await InsertDecodedCaptchaInField();
            AuthorizationOnSite(SearchMethod.Id, "signup_form_email", "signup_form_password", "signup_button", LOGIN, PASSWORD);
            Thread.Sleep(2000);

            bool isErrorCapthca = true;
            while (isErrorCapthca)
            {
                isErrorCapthca = GetElementByXPath("//*[@id='reward_point_redeem_result_container_div']").Displayed;
                if (!isErrorCapthca)
                {
                    _ruCaptchaController.SendReport(_ruCaptchaController.GetCaptchaQueryId(), "reportgood");
                    break;
                }

                _ruCaptchaController.SendReport(_ruCaptchaController.GetCaptchaQueryId(), "reportbad");
                await InsertDecodedCaptchaInField();
                GetElementById("signup_button").Click();
            }

            GoToUrl(urlCrane);
            Thread.Sleep(2000);

            var advertisingWindow = GetElementByXPath("//*[@id='push_notification_modal']/div[1]/div[2]/div/div[1]");
            if (advertisingWindow.Displayed)
            {
                advertisingWindow.Click();
            }
        }
        /// <summary>
        /// Вставить расшифрованную капчу в поле
        /// </summary>
        /// <returns>Асинхронная задача</returns>
        protected async Task InsertDecodedCaptchaInField()
        {
            string imageSrc = GetElementByXPath("//*[@id='botdetect_signup_captcha']/div[1]/img").GetSrc();
            OpenPageInNewTab(imageSrc);

            string imageByte = ConvertImageToByte();
            string responseOnCaptcha = ERROR_BAD_DUPLICATES;

            while (responseOnCaptcha == ERROR_BAD_DUPLICATES)
            {
                responseOnCaptcha = await _ruCaptchaController.SendCaptchaImage(imageByte);
            }

            GetElementByXPath("//*[@id='botdetect_signup_captcha']/input[2]").SendKeys(responseOnCaptcha);
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
        /// Конвертация изображения в байты
        /// </summary>
        /// <param name="imageSrc">Изображение</param>
        /// <returns>Строка байтов</returns>
        protected string ConvertImageToByte()
        {
            string result = ExecuteScript(
            "var c = document.createElement(\"canvas\");" +
            "var ctx = c.getContext(\"2d\");" +
            "var img = document.querySelector(\"body > img\");" +
            "c.height = img.height;" +
            "c.width = img.width;" +
            "ctx.drawImage(img, 0, 0);" +
            "return c.toDataURL(\"image/jpeg\");").Replace("data:image/jpeg;base64,", string.Empty);

            CloseTab();
            SwitchToTab();

            return result;
        }
        /// <summary>
        /// Проверка страницы
        /// </summary>
        /// <param name="url">Страница</param>
        /// <returns>True если открыта нужная страница, иначе - false</returns>
        protected bool CheckPage(string url)
        {
            return GetUrlPage() == url ? true : false;
        }
        /// <summary>
        /// Существует ли таймер
        /// </summary>
        /// <returns>True если есть время внутри элемента, иначе - false</returns>
        protected bool IsTimerExist()
        {
            string timer = GetElementsByXPath("//*[@id='time_remaining']").First().Text;
            if (!string.IsNullOrEmpty(timer))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Получить таймер
        /// </summary>
        /// <returns>Время таймера</returns>
        protected TimeSpan GetTimer()
        {
            string timer = "00:";

            timer += ExecuteScript("return document.getElementById('time_remaining').innerText;");
            timer = timer.Replace("Minutes", ":").Replace("Minute", string.Empty)
                         .Replace("Seconds", string.Empty).Replace("Second", string.Empty)
                         .Replace("\r", string.Empty).Replace("\n", string.Empty);

            return TimeSpan.Parse(timer == "00:60:0" ? timer = "00:59:59" : timer);
        }
        /// <summary>
        /// Получить баланс на кране
        /// </summary>
        /// <returns>Баланс</returns>
        protected string BalanceCrane()
        {
            return GetElementByXPath("//*[@id='balance']", 15).Text;
        }
        /// <summary>
        /// Получить подробности с крана
        /// </summary>
        /// <param name="crane">Модель крана</param>
        /// <returns>Модель с обновленными данными</returns>
        protected Crane GetDetailsWithCrane(Crane crane)
        {
            crane.ActivityTime = GetTimer();
            crane.BalanceOnCrane = BalanceCrane();

            CloseTab();

            return crane;
        }
        /// <summary>
        /// Расшифровать капчу
        /// </summary>
        /// <param name="token">Токен рекапчи</param>
        /// <param name="urlCrane">Url-адрес крана</param>
        /// <returns>Расшифрованный токен капчи или ошибку от сервиса RuCaptcha</returns>
        protected async Task DecipherCaptcha(string urlCrane, string xPath)
        {
            string token = GetElementByXPath(xPath).GetDataSitekey();
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
            if (!GetElementById("free_play_error").Displayed)
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
