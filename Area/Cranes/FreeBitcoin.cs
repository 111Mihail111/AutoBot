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
        const string LOGIN = "polowinckin.mixail@yandex.ru";
        const string PASSWORD = "xHkKv78SvV2o7rSX";
        private RuCaptchaController _ruCaptchaController = new RuCaptchaController();


        public async Task<Crane> GoTo(Crane crane)
        {
            string urlCrane = crane.URL;

            GoToUrl(urlCrane);
            await AuthorizationOnCrane(urlCrane);

            Thread.Sleep(2000);
            GetElementByXPath("/html/body/div[24]").Click(); //Закрытие рекламного окна
            GetElementByXPath("/html/body/div[1]/div/a[1]").Click(); //Подтверждение о куках сайта
            SetScrollPosition(1000);

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
                string imageByte = GetDataCaptcha(Captcha.RegularCaptcha);
                string responseOnCaptcha = await _ruCaptchaController.SendCaptcha(imageByte);

                while (isAuthorization == false)
                {
                    GetElementByXPath("//*[@id='botdetect_signup_captcha']/input[2]").SendKeys(responseOnCaptcha);
                    AuthorizationOnCrane("signup_form_email", "signup_form_password", "signup_button", LOGIN, PASSWORD);
                    Thread.Sleep(1000);
                    isAuthorization = CheckPage(urlCrane);
                }

                GoToUrl(urlCrane);
            }
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
                    return GetTokenCaptcha(url);
                case Captcha.RegularCaptcha:
                    var imageSrc = GetElementByXPath("//*[@id='botdetect_signup_captcha']/div[1]/img").GetAttribute("src");
                    GoToUrlNewTab(imageSrc);
                    return ConvertImageToByte();
            }

            return string.Empty;
        }
        /// <summary>
        /// Получить токен капчи
        /// </summary>
        /// <param name="url">Url-адрес капчи</param>
        /// <returns>Токен</returns>
        public string GetTokenCaptcha(string url)
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
            return Browser.Url == url || Browser.Url == "https://freebitco.in/?op=home" ? true : false;
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
        public string GetTimer()
        {
            var timer = GetElementByXPath("//*[@id='time_remaining']/span/").Text;
            timer = timer.Replace("Minutes", ":").Replace("Seconds", "");

            return TimeSpan.Parse(timer).ToString(@"hh\:mm\:ss");
        }
        /// <summary>
        /// Получить баланс на кране
        /// </summary>
        /// <returns>Баланс</returns>
        public double BalanceCrane()
        {
            return Convert.ToDouble(GetElementByXPath("//*[@id='balance']").Text);
        }
        /// <summary>
        /// Получить подробности с крана
        /// </summary>
        /// <param name="crane">Модель крана</param>
        /// <returns>Модель с обновленными данными</returns>
        public Crane GetDetailsWithCrane(Crane crane)
        {
            crane.ActivityTime = TimeSpan.Parse(GetTimer());
            crane.BalanceOnCrane = BalanceCrane();

            return crane;
        }
    }
}
