using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Extentions;
using AutoBot.Models;
using System;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class V_Like : BrowserManager, IV_Like
    {
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\PerformanceTasks\\V_Like\\";
        const string LOGIN = "89524349046"; //TODO: Настройки вынести отдельно на страницу
        const string PASSWORD = "123qwerzxcv";  //TODO: Настройки вынести отдельно на страницу
        const string LOGIN_INSTAGRAM = "fedosinalexei@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        const string PASSWORD_INSTAGRAM = "123q_Q*W(*E&*R^*Z$*X!*C?*V";  //TODO: Настройки вынести отдельно на страницу

        public InternetService GoTo(InternetService service)
        {
            Initialization(BROWSER_PROFILE_CRANE);
            GoToUrl(service.URL);
            AuthorizationOnService();
            BeginCollecting();

            return GetDetailsWithService(service);
        }

        /// <summary>
        /// Начать сбор
        /// </summary>
        private void BeginCollecting()
        {
            JoinInCommunityVK();
            //WorkWithLikesVK();
            //AddToFriendsVK();
            //SubscriptionsInInstagram();
            //WorkWithLikeInstagram();
        }

        /// <summary>
        /// Вступить в сообщество ВК
        /// </summary>
        protected void JoinInCommunityVK()
        {
            GetElementByXPath("//*[@id='vk1']/a").Click(); //Вступление в ВК
            
            var message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            while (message == null)
            {
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                
                if (GetTabsCount() > 1)
                {
                    SwitchToLastTab();
                    CloseTab();
                    SwitchToTab();
                }

                GetElementByXPath("//*[@id='content']/p[4]/a").Click();
                SwitchToLastTab();

                if (IsPrivateCommunityVK())
                {
                    CloseTab();
                    SwitchToTab();
                    GetElementByXPath("//*[@id='buttons']/a[1]").Click();
                    GetElementByXPath("//*[@id='content']/div[3]/div[1]/a[1]").Click();
                    continue;
                }

                GetElementById("join_button")?.Click();
                GetElementById("public_subscribe")?.Click();

                Thread.Sleep(1000);

                CloseTab();
                SwitchToTab();
                GetElementByXPath("//*[@id='buttons']/a[2]").Click();

                message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            }
            
        }
        /// <summary>
        /// Работа с лайками
        /// </summary>
        protected void WorkWithLikesVK()
        {
            GetElementByXPath("//*[@id='vk2']/a").Click();

            var perfomanse = GetElementByClassName("groups").GetInnerText();
            while (perfomanse != string.Empty && perfomanse != "Нет доступных заданий. Заходите позже!")
            {
                ButtonsVisible();

                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                var title = GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[2]/p").GetInnerText();
                SwitchToLastTab();

                if (IsError())
                {
                    CloseTab();
                    SwitchToTab();
                    GetElementByXPath("//*[@id='buttons']/a[1]").Click();
                    GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/span").Click();
                    AlertAccept();
                    continue;
                }
                
                if (title == "Поставить Лайк + Рассказать друзьям")
                {
                    LikeIt();
                    MakeRepost(); //TODO:Пока не отлажен
                }
                else
                {
                    LikeIt();
                }

                CloseTab();
                SwitchToTab();
                Thread.Sleep(5000);
                GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                Thread.Sleep(1000);
                perfomanse = GetElementByClassName("groups").GetInnerText();
            }
        }
        /// <summary>
        /// Добавить в друзья
        /// </summary>
        protected void AddToFriendsVK()
        {
            GetElementByXPath("//*[@id='vk3']/a").Click();

            var message = GetElementByXPath("//*[@id='content']/div[2]/h3").GetInnerText();
            while (message != "Никто не ищет друзей :-(")
            {
                ButtonsVisible();
                GetElementByXPath("//*[@id='friend20321']/div[3]/a").Click();
                SwitchToLastTab();
                GetElementByXPath("//*[@id='friend_status']/div/button").Click();
                CloseTab();
                SwitchToTab();
                GetElementByXPath("//*[@id='buttons']/a[2]");

                message = GetElementByXPath("//*[@id='content']/div[2]/h3").GetInnerText();
            }
        }
        /// <summary>
        /// Подписки в инстаграмм
        /// </summary>
        protected void SubscriptionsInInstagram()
        {
            GetElementByXPath("//*[@id='in0']/a").Click();

            var groups = GetElementsByClassName("groups");
            while (groups.Count() != 0)
            {
                GetElementByXPath("//*[@id='content']/div[2]/div[1]/div[3]/a").Click();
                SwitchToLastTab();
                var subscribe = GetElementByTagName("button");

                if (subscribe == null)
                {
                    CloseTab();
                    SwitchToTab();
                    GetElementByXPath("//*[@id='buttons']/a[1]").Click();
                    Thread.Sleep(1500);
                    GetElementByXPath("//*[@id='content']/div[2]/div[1]/a[1]").Click();
                    continue;
                }
                subscribe.Click();

                var loginForm = GetElementById("loginForm");
                if (loginForm != null && loginForm.Displayed)
                {
                    AuthorizationInInstagram();
                }

                GetElementByTagName("button").Click();
                CloseTab();
                SwitchToTab();
                Thread.Sleep(1000);
                GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                Thread.Sleep(2000);
                groups = GetElementsByClassName("groups");
            }
        }
        protected void WorkWithLikeInstagram()
        {
            GetElementByXPath("//*[@id='in1']/a").Click();
        }

        protected void AuthorizationOnService()
        {
            var button = GetElementByXPath("//*[@id='uLogin']/div");
            if (button == null)
            {
                return;
            }

            button.Click();
            Thread.Sleep(2000);

            if (GetTabsCount() > 1)
            {
                SwitchToLastTab();
                InsertLoginAndPassword();
                SwitchToTab();
            }
        }
        protected void AuthorizationInInstagram()
        {
            GetElementByXPath("//*[@id='loginForm']/div[1]/div[1]/div/label/input").SendKeys(LOGIN_INSTAGRAM);
            GetElementByXPath("//*[@id='loginForm']/div[1]/div[2]/div/label/input").SendKeys(PASSWORD_INSTAGRAM);
            GetElementByXPath("//*[@id='loginForm']/div[1]/div[3]/button/div").Click();
            GetElementByXPath("//*[@id='react-root']/section/main/div/div/div/section/div/button").Click();
        }

        protected void InsertLoginAndPassword() //TODO:Метод будет асинхронным
        {
            GetElementByXPath("//*[@id='login_submit']/div/div/input[6]").SendKeys(LOGIN); //TODO:Создать 2 новых метода. Второй асинхронный
            GetElementByXPath("//*[@id='login_submit']/div/div/input[7]").SendKeys(PASSWORD); //TODO:Создать 2 новых метода. Второй асинхронный
            GetElementById("install_allow").Click();
        }

        /// <summary>
        /// Частное сообщество
        /// </summary>
        /// <returns></returns>
        protected bool IsPrivateCommunityVK()
        {
            var message = GetElementByXPath("//*[@id='page_info_wrap']/div/div/div")?.GetInnerText();
            string text = "Это частное сообщество. Доступ только по приглашениям администраторов.";

            if (message == text)
            {
                return true;
            }

            return false;
        }

        protected bool IsError()
        {
            var errorMessage = GetElementByXPath("//*[@id='content']/div/div[1]").GetInnerText();
            if (errorMessage == "Ошибка")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Поставить лайк
        /// </summary>
        protected void LikeIt()
        {
            var post = GetUrlPage().Replace("https://vk.com/wall", "post");
            SetScrollPosition(10000);
            GetElementByXPath($"//*[@id='{post}']/div/div[2]/div/div[2]/div/div[1]/a[1]/div[1]").Click();
        }
        /// <summary>
        /// Сделать репост
        /// </summary>
        protected void MakeRepost()
        {
            var post = GetUrlPage().Replace("https://vk.com/wall", "post");
            SetScrollPosition(10000);
            GetElementByXPath($"//*[@id='{post}']/div/div[2]/div/div[2]/div/div[1]/a[2]/div[1]").Click();
            GetElementById("like_share_my").Click();
            GetElementById("like_share_send").Click();
        }
        /// <summary>
        /// Кнопки видимы
        /// </summary>
        protected void ButtonsVisible()
        {
            ExecuteScript("var buttonList = document.getElementsByClassName('button');" +
                "for (var i = 0; i < buttonList.length; i++)" +
                "{" +
                    "buttonList[i].style.display = 'inline-block';" +
                "}");
        }
        /// <summary>
        /// Получить детали интернет-сервиса
        /// </summary>
        /// <param name="internetService">Интернет-сервис</param>
        /// <returns>Модель интернет-сервиса</returns>
        protected InternetService GetDetailsWithService(InternetService internetService)
        {
            internetService.ActivityTime = TimeSpan.FromMinutes(10);
            internetService.BalanceOnService = GetElementByClassName("balance").GetInnerText();
            internetService.StatusService = Status.Work;

            QuitBrowser();

            return internetService;
        }
    }
}
