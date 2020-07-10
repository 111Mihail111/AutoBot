using AutoBot.Area.API;
using AutoBot.Area.Interface;
using AutoBot.Area.Managers;
using AutoBot.Enums;
using AutoBot.Extentions;
using AutoBot.Models;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.Cranes
{
    public class FreeBitcoin : BrowserManager, IFreeBitcoin
    {
        private RuCaptchaController _ruCaptchaController = new RuCaptchaController(); //TODO: Обернуть интерфейсом и прокинуть через DI
        const string LOGIN = "polowinckin.mixail@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        const string PASSWORD = "xHkKv78SvV2o7rSX";  //TODO: Настройки вынести отдельно на страницу

        public async Task<Crane> GoTo(Crane crane)
        {
            string urlCrane = crane.URL;

            GoToUrl(urlCrane);
            Thread.Sleep(2000);
            await AuthorizationOnCrane(urlCrane);
            SetScrollPosition(1000);

            if (IsTimerExist())
            {
                return GetDetailsWithCrane(crane);
            }

            string token = GetElementByXPath("//*[@id='free_play_recaptcha']/form/div").GetDataSitekey();
            string responseOnCaptcha = await DecipherCaptcha(token, urlCrane);

            if (responseOnCaptcha == ERROR_CAPTCHA_UNSOLVABLE)
            {
                return await GoTo(crane);
            }
            
            GetElementById("free_play_form_button").Click();
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
            bool isAuthorization = CheckPage(urlCrane);
            if (!isAuthorization)
            {
                GetElementByXPath("/html/body/div[1]/div/a[1]").Click(); //Подтверждение о куках сайта
                await InsertDecodedCaptchaInField();
                AuthorizationOnSite(SearchMethod.Id, "signup_form_email", "signup_form_password", "signup_button", LOGIN, PASSWORD);
                Thread.Sleep(2000);

                bool isErrorCapthca = true;
                while (isErrorCapthca)
                {
                    isErrorCapthca = GetElementByXPath("//*[@id='reward_point_redeem_result_container_div']").Displayed;
                    if (!isErrorCapthca)
                    {
                        _ruCaptchaController.SendReportOnCaptcha(_ruCaptchaController.GetKeyCaptcha(), "reportgood");
                        break;
                    }

                    _ruCaptchaController.SendReportOnCaptcha(_ruCaptchaController.GetKeyCaptcha(), "reportbad");
                    await InsertDecodedCaptchaInField();
                    GetElementById("signup_button").Click();
                }
            }

            GoToUrl(urlCrane);
            Thread.Sleep(3000);

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
            string responseOnCaptcha = await _ruCaptchaController.SendCaptcha(imageByte);

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

            return crane;
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
