using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.Managers.Interface;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Extentions;
using AutoBot.Models;
using Microsoft.CodeAnalysis;
using OpenQA.Selenium.Interactions;
using System;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class VLike : BrowserManager, IVLike
    {
        private IVkManager _vkManager;
        private IInstagramManager _instaManager;

        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\PerformanceTasks\\V_Like\\";
        private static bool _isAuthorization;

        protected void Init()
        {
            Initialization(BROWSER_PROFILE_CRANE);
            SetContextForManagers();

            if (!_isAuthorization)
            {
                AuthorizationSocialNetworks();
            }
        }

        /// <summary>
        /// Установить контекст для менеджеров
        /// </summary>
        protected void SetContextForManagers()
        {
            _vkManager = new VkManager();
            _instaManager = new InstagramManager();

            var driver = GetDriver();
            _vkManager.SetContextBrowserManager(driver);
            _instaManager.SetContextBrowserManager(driver);
        }

        /// <summary>
        /// Авторизация в соц. сетях
        /// </summary>
        protected void AuthorizationSocialNetworks()
        {
            var accounts = AccountService.GetAccount(TypeService.VLike);
            var accountVK = accounts.First();
            var accountInstagram = accounts.Where(w => w.AccountType == AccountType.Instagram).First();

            _vkManager.Authorization(accountVK.Login, accountVK.Password);
            _instaManager.Authorization(accountInstagram.Login, accountInstagram.Password);
            _isAuthorization = true;
        }

        public InternetService GoTo(InternetService service)
        {
            Init();
            GoToUrl(service.URL);
            BeginCollecting();

            return GetDetailsWithService(service);
        }

        /// <summary>
        /// Начать сбор
        /// </summary>
        private void BeginCollecting() //ЕСТЬ TODO
        {
            var login = GetElementByXPath("//*[@id='uLogin']/div");
            if (login != null)
            {
                login.Click();
            }

            JoinInCommunityVK();
            WorkWithLikesVK();
            //AddToFriendsVK();
            //WorkWithYouTube(); //TODO: Не отлажен
            SubscriptionsInInstagram();
            WorkWithLikeInstagram();
        }


        /// <summary>
        /// Вступить в сообщество ВК
        /// </summary>
        protected void JoinInCommunityVK() //Есть TODO
        {
            GetElementByXPath("//*[@id='vk1']/a").Click();

            var message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            while (message == null)
            {
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                SwitchToLastTab();
                _vkManager.RemoveWindowMessage();

                string url = GetUrlPage();

                if (_vkManager.IsPrivateGroup() || _vkManager.IsBlockedCommunity())
                {
                    SkipTask("vkCommunity");
                    message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
                    continue;
                }

                _vkManager.JoinToComunity();

                CloseTab();
                SwitchToTab();

                if (DelayPayments())
                {
                    OpenPageInNewTab(url);
                    _vkManager.UnsubscribeToComunity(); //TODO:Протестировать
                    SkipTask("vkCommunity");
                }

                message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            }
        }

        /// <summary>
        /// Работа с лайками
        /// </summary>
        protected void WorkWithLikesVK() //Есть TODO
        {
            GetElementByXPath("//*[@id='vk2']/a").Click();

            var perfomanse = GetElementByClassName("groups")?.GetInnerText();
            while (!string.IsNullOrEmpty(perfomanse) && perfomanse != "Нет доступных заданий. Заходите позже!")
            {
                ButtonsVisible();
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                var titleTask = GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[2]/p").GetInnerText();

                SwitchToLastTab();
                _vkManager.RemoveWindowMessage();
                RefreshPage();

                //TODO: Проверка вдруг запись не найдена https://vk.com/wall-35192468_75363

                if (titleTask == "Поставить Лайк + Рассказать друзьям")
                {
                    _vkManager.MakeRepost();
                }

                _vkManager.PutLike(); 
                Thread.Sleep(3500);

                string url = GetUrlPage();
                CloseTab();
                SwitchToTab();

                GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                Thread.Sleep(1000);

                if (IsMaxLikes())
                {
                    _vkManager.RemoveLike(url);
                    SkipTask("vkLike");
                    perfomanse = GetElementByClassName("groups").GetInnerText();
                    continue;
                }
                else if (GetTextFromAlert() == "Лайк не был поставлен")
                {
                    if (DelayPayments())
                    {
                        _vkManager.RemoveLike(url);
                        GetElementByXPath("//*[@id='buttons']/a[1]").Click();
                        Thread.Sleep(1000);
                        GetElementByClassName("groups").FindElements(SearchMethod.ClassName, "group").First()
                            .FindElement(SearchMethod.Tag, "a").Click();
                        AlertAccept();
                    }
                }

                perfomanse = GetElementByClassName("groups").GetInnerText();
            }
        }

        /// <summary>
        /// Задержка платежа
        /// </summary>
        public bool DelayPayments()
        {
            try
            {
                int counter = 0;
                var modal = GetElementById("modal");
                while (modal.Displayed)
                {
                    GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                    Thread.Sleep(5000);
                    AlertAccept();

                    counter++;
                    if (counter == 10)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                //Alert'a нет, значит модал. окно закрылось
            }

            return false;
        }
        /// <summary>
        /// Пропустить задание
        /// </summary>
        public void SkipTask(string typeTask)
        {
            CloseTab();
            SwitchToTab();
            GetElementByXPath("//*[@id='buttons']/a[1]").Click();
            Thread.Sleep(1000);

            switch (typeTask)
            {
                case "vkLike":
                    GetElementByXPath("//*[@id='content']/div[3]/div/div[3]/span").Click();
                    AlertAccept();
                    break;
                case "instaSubscription":
                case "instaLike":
                    GetElementByXPath("//*[@id='content']/div[2]/div/a[1]").Click();
                    AlertAccept();
                    break;
                case "vkCommunity":
                    GetElementByXPath("//*[@id='content']/div[3]/div/a[1]").Click();
                    break;
            }

            Thread.Sleep(2000);
        }

        /// <summary>
        /// Максимум лайков
        /// </summary>
        /// <returns>True - да, иначе false</returns>
        protected bool IsMaxLikes()
        {
            var massage = GetTextFromAlert();
            if (massage == "К сожалению, уже было поставлено нужное количество лайков к данному объекту. Обновите список заданий.")
            {
                AlertAccept();
                return true;
            }

            return false;
        }

        protected void WorkWithYouTube() //ЕСТЬ TODO
        {
            GetElementByXPath("//*[@id='yt0']/a").Click();

            var groups = GetElementByClassName("groups");
            while (true) //TODO:Проверка на отсутствие заданий
            {
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                SwitchToLastTab();

                if (IsVideoAvailable())
                {
                    SkipTask("");
                    groups = GetElementByClassName("groups");
                    continue;
                }

                GetElementById("button").Click();
                CloseTab();
                SwitchToTab();
                GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                DelayPayments();

                groups = GetElementByClassName("groups");
            }
        }

        /// <summary>
        /// Доступно ли видео
        /// </summary>
        /// <returns>True - не доступно, иначе false</returns>
        protected bool IsVideoAvailable()
        {
            return GetElementById("reason") != null;
        }

        /// <summary>
        /// Добавить в друзья
        /// </summary>
        protected void AddToFriendsVK()
        {
            GetElementByXPath("//*[@id='vk3']/a").Click();

            var message = GetElementByXPath("//*[@id='content']/div[2]/h3").GetInnerText(); ////*[@id="content"]/div[2]
            while (message != "Никто не ищет друзей :-(")
            {
                ButtonsVisible();
                GetElementByXPath("//*[@id='content']/div[2]/div[2]/div[3]/a").Click();
                SwitchToLastTab();

                //RemoveWindowMessage();

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
                Thread.Sleep(1000);

                if (!_instaManager.IsFoundPage())
                {
                    SkipTask("instaSubscription");
                    groups = GetElementsByClassName("groups");
                    continue;
                }

                _instaManager.Subscribe();
                string url = GetUrlPage();

                CloseTab();
                SwitchToTab();

                if (DelayPayments())
                {
                    OpenPageInNewTab(url);
                    _instaManager.Unsubscribe();
                    SkipTask("instaSubscription");
                }

                groups = GetElementsByClassName("groups");
            }
        }

        /// <summary>
        /// Лайки в инстаграмме
        /// </summary>
        protected void WorkWithLikeInstagram()
        {
            GetElementByXPath("//*[@id='in1']/a").Click();

            var groups = GetElementsByClassName("groups");
            while (groups.Count() != 0)
            {
                GetElementByXPath("//*[@id='content']/div[2]/div[1]/div[3]/a").Click();
                SwitchToLastTab();
                Thread.Sleep(2500);

                if (!_instaManager.IsFoundPage())
                {
                    SkipTask("instaLike");
                    groups = GetElementsByClassName("groups");
                    continue;
                }

                _instaManager.PutLike();
                string url = GetUrlPage();

                CloseTab();
                SwitchToTab();
               
                if (DelayPayments())
                {
                    OpenPageInNewTab(url);
                    _instaManager.RemoveLike();
                    SkipTask("instaLike");
                    groups = GetElementsByClassName("groups");
                    continue;
                }

                groups = GetElementsByClassName("groups");
            }
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
            internetService.ActivityTime = TimeSpan.FromMinutes(1);
            internetService.BalanceOnService = GetElementByClassName("balance").GetInnerText();
            internetService.StatusService = Status.Work;

            QuitBrowser();

            return internetService;
        }
    }
}
