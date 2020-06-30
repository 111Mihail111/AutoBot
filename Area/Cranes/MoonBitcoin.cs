using AutoBot.Area.API;
using AutoBot.Area.Managers;
using AutoBot.Extentions;
using AutoBot.Models;
using System.Threading;

namespace AutoBot.Area.Cranes
{
    public class MoonBitcoin : BrowserManager
    {
        private RuCaptchaController _ruCaptchaController = new RuCaptchaController(); //TODO: Обернуть интерфейсом и прокинуть через DI
        const string LOGIN = "polowinckin.mixail@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        

        public async void GoTo(Crane crane)
        {
            string urlCrane = crane.URL;

            GoToUrl(urlCrane);

            if (GetUrlPage() != urlCrane)
            {
                GetElementByXPath("//*[@id='PageContent_UnauthorisedButtons']/button").Click();
                Thread.Sleep(2000);
                GetElementById("SignInEmailInput").SendKeys(LOGIN);

                string token = GetElementByXPath("//*[@id='free_play_recaptcha']/form/div").GetDataSitekey();
                string responseOnCaptcha = await _ruCaptchaController.SendCaptcha_v2(token, urlCrane);
            }

        }
    }
}
