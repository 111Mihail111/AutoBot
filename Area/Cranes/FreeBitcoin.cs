using AutoBot.Area.API;
using AutoBot.Area.Managers;
using AutoBot.Enums;
using AutoBot.Models;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Text;

namespace AutoBot.Area.Cranes
{
    public class FreeBitcoin : BrowserManager
    {
        const string LOGIN = "polowinckin.mixail@yandex.ru";
        const string PASSWORD = "xHkKv78SvV2o7rSX";

        public void GoTo(Crane crane)
        {
            string urlCrane = crane.URL;

            GoToUrl(urlCrane);

            if (!CheckPage(urlCrane))
            {
                string imageByte = GetCaptcha(Captcha.RegularCaptcha);
                //TODO: Отправка капчи на сервак 
                RuCaptchaController ruCaptchaController = new RuCaptchaController();
                ruCaptchaController.SendCaptcha(imageByte);

                AuthorizationOnCrane("signup_form_email", "signup_form_password", "signup_button", LOGIN, PASSWORD);
                GoToUrl(urlCrane);
            }

            //Закрытие рекламного окна
            GetElementByXPath("/html/body/div[24]").Click();

            //Подтверждение о куках сайта
            GetElementByXPath("/html/body/div[1]/div/a[1]").Click();

            SetScrollPosition(1000);

            if (IsTimerExist())
            {
                crane.ActivityTime = TimeSpan.Parse(GetTimer());
                crane.BalanceOnCrane = BalanceCrane();
            }

            string token = GetCaptcha(Captcha.ReCaptcha_V2);
            //TODO: Отправка капчи на сервак

        }


        /// <summary>
        /// Получить капчу
        /// </summary>
        /// <param name="captcha">Тип капчи</param>
        /// <returns>Данные для API запроса</returns>
        public string GetCaptcha(Captcha captcha)
        {
            switch (captcha)
            {
                case Captcha.ReCaptcha_V2:
                    var url = GetElementByXPath("//*[@id='free_play_recaptcha']/form/div/div/div/iframe").GetAttribute("src");
                    return GetTokenCaptcha(url);
                case Captcha.RegularCaptcha:
                    var imageSrc = GetElementByXPath("//*[@id='botdetect_signup_captcha']/div[1]/img").GetAttribute("src");
                    return ConvertImageToByte(imageSrc);
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
        public string ConvertImageToByte(string imageSrc)
        {
            var bytes = Encoding.UTF8.GetBytes(imageSrc);
            return Convert.ToBase64String(bytes);
        }
        /// <summary>
        /// Проверка страницы
        /// </summary>
        /// <param name="url">Страница</param>
        /// <returns>True если открыта нужная страница, иначе - false</returns>
        public bool CheckPage(string url)
        {
            if (Browser.Url == url)
            {
                return true;
            }

            return false;
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
    }
}
