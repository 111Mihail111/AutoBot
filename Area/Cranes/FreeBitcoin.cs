using AutoBot.Area.API;
using AutoBot.Area.Interface;
using AutoBot.Area.Managers;
using AutoBot.Enums;
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
            SetScrollPosition(0, 1000);

            if (IsTimerExist())
            {
                return GetDetailsWithCrane(crane);
            }

            string token = GetElementByXPath("//*[@id='free_play_recaptcha']/form/div").GetAttribute("data-sitekey");
            string responseOnCaptcha = await _ruCaptchaController.SendCaptcha_v2(token, urlCrane);

            HiddenFieldVisible("g-recaptcha-response");
            GetElementByXPath("//*[@id='g-recaptcha-response']").SendKeys(responseOnCaptcha);
            HiddenFieldInVisible("g-recaptcha-response");
            GetElementById("free_play_form_button").Click(); // Нажать на ROLL

            return GetDetailsWithCrane(crane);
        }


        public async Task AuthorizationOnCrane(string urlCrane)
        {
            bool isAuthorization = CheckPage(urlCrane);
            if (!isAuthorization)
            {
                GetElementByXPath("/html/body/div[1]/div/a[1]").Click(); //Подтверждение о куках сайта
                await InsertToField();
                AuthorizationOnSite("signup_form_email", "signup_form_password", "signup_button", LOGIN, PASSWORD);
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
                    await InsertToField();
                    GetElementById("signup_button").Click();
                }
            }

            GoToUrl(urlCrane);
            Thread.Sleep(5000);

            var advertisingWindow = GetElementByXPath("//*[@id='push_notification_modal']/div[1]/div[2]/div/div[1]");
            if (advertisingWindow.Displayed)
            {
                advertisingWindow.Click();
            }
        }

        public async Task InsertToField()
        {
            string imageByte = GetDataCaptcha(Captcha.RegularCaptcha);
            string responseOnCaptcha = await _ruCaptchaController.SendCaptcha(imageByte);

            GetElementByXPath("//*[@id='botdetect_signup_captcha']/input[2]").SendKeys(responseOnCaptcha);
        }

        public void HiddenFieldVisible(string xPath)
        {
            Browser.ExecuteScript($"var textArea = document.getElementById('{xPath}');" +
                "textArea.style.position = 'absolute';" +
                "textArea.style.display = 'inline';");
        }
        public void HiddenFieldInVisible(string xPath)
        {
            Browser.ExecuteScript($"var textArea = document.getElementById('{xPath}');" +
                "textArea.style.position = '';" +
                "textArea.style.display = 'none';");
        }

        /// <summary>
        /// Получить капчу
        /// </summary>
        /// <param name="captcha">Тип капчи</param>
        /// <returns>Данные для API запроса</returns>
        public string GetDataCaptcha(Captcha captcha)
        {
            switch (captcha)
            {
                case Captcha.ReCaptcha_V2:
                    var url = GetElementByXPath("//*[@id='free_play_recaptcha']/form/div").GetAttribute("data-sitekey");
                    return GetTokenReCaptcha(url);
                case Captcha.RegularCaptcha:
                    var imageSrc = GetElementByXPath("//*[@id='botdetect_signup_captcha']/div[1]/img").GetAttribute("src");
                    OpenPageInNewTab(imageSrc);
                    return ConvertImageToByte();
            }

            return string.Empty;
        }
        /// <summary>
        /// Получить токен Рекапчи
        /// </summary>
        /// <param name="url">Url-адрес капчи</param>
        /// <returns>Токен</returns>
        public string GetTokenReCaptcha(string url)
        {
            var array = url.Split('&');
            string token = array[1].Trim('k', '=');

            return token;
        }
        /// <summary>
        /// Конвертация изображения в байты
        /// </summary>
        /// <param name="imageSrc">Изображение</param>
        /// <returns>Строка байтов</returns>
        public string ConvertImageToByte()
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
        public bool CheckPage(string url)
        {
            return GetUrlPage() == url ? true : false;
        }
        /// <summary>
        /// Существует ли таймер
        /// </summary>
        /// <returns>True если есть время внутри элемента, иначе - false</returns>
        public bool IsTimerExist()
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
        public TimeSpan GetTimer()
        {
            string timer = "00:";
            timer += GetElementByXPath("//*[@id='time_remaining']/span", 15).Text;
            timer = timer.Replace("Minutes", ":").Replace("Minute", string.Empty)
                         .Replace("Seconds", string.Empty).Replace("Second", string.Empty)
                         .Replace("\r", string.Empty).Replace("\n", string.Empty);

            return TimeSpan.Parse(timer == "00:60:0" ? timer + "0" : timer);
        }
        /// <summary>
        /// Получить баланс на кране
        /// </summary>
        /// <returns>Баланс</returns>
        public string BalanceCrane()
        {
            return GetElementByXPath("//*[@id='balance']", 15).Text;
        }
        /// <summary>
        /// Получить подробности с крана
        /// </summary>
        /// <param name="crane">Модель крана</param>
        /// <returns>Модель с обновленными данными</returns>
        public Crane GetDetailsWithCrane(Crane crane)
        {
            crane.ActivityTime = GetTimer();
            crane.BalanceOnCrane = BalanceCrane();

            return crane;
        }
    }
}
