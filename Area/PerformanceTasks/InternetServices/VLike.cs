using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.Managers.Interface;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Extentions;
using AutoBot.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class VLike : BrowserManager, IVLike
    {
        private IVkManager _vkManager;
        private IInstagramManager _instaManager;
        private ILogManager _logManager;

        const string BROWSER_PROFILE_CRANE = "C:\\_AutoBot\\Profiles\\PerformanceTasks\\V_Like\\";
        private static bool _isAuthorization;
        private string _taskId;

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
            _logManager = new LogManager();

            var driver = GetDriver();
            _vkManager.SetContextBrowserManager(driver);
            _instaManager.SetContextBrowserManager(driver);
        }
        /// <summary>
        /// Авторизация в соц. сетях
        /// </summary>
        protected void AuthorizationSocialNetworks()
        {
            var accounts = AccountService.GetAccountsByType(TypeService.VLike);

            var accountVK = accounts.Where(w => w.AccountType == AccountType.Main).FirstOrDefault();
            if (accountVK != null)
            {
                _vkManager.Authorization(accountVK.Login, accountVK.Password);
            }

            var accountInstagram = accounts.Where(w => w.AccountType == AccountType.Instagram).FirstOrDefault();
            if (accountInstagram != null)
            {
                _instaManager.Authorization(accountInstagram.Login, accountInstagram.Password);
            }

            _isAuthorization = true;
        }


        public InternetService GoTo(InternetService service)
        {
            //try
            //{
                Init();
                GoToUrl(service.URL);
                AuthorizationOnService();
                BeginCollecting();

                return GetDetailsWithService(service);
            //}
            //catch (NullReferenceException nullReferenceException)
            //{
            //    _logManager.SaveDetailsException(nullReferenceException);
            //}

            //return service;
        }

        /// <summary>
        /// Начать сбор
        /// </summary>
        private void BeginCollecting() //ЕСТЬ TODO
        {
            JoinInCommunityVK();
            WorkWithLikesAndRepostVK();
            //AddToFriendsVK();
            //WorkWithYouTube(); //TODO: Не отлажен
            SubscriptionsInInstagram();
            WorkWithLikeInstagram();
        }


        /// <summary>
        /// Вступить в сообщество ВК
        /// </summary>
        protected void JoinInCommunityVK()
        {
            GetElementById("vk1").Click();

            var message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            while (message == null)
            {
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                SwitchToLastTab();

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
                    _vkManager.UnsubscribeToComunity();
                    SkipTask("vkCommunity");
                }

                message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            }
        }
        /// <summary>
        /// Работа с лайками
        /// </summary>
        protected void WorkWithLikesAndRepostVK()
        {
            GetElementById("vk2").Click();

            var perfomanse = GetElementByClassName("groups")?.GetInnerText();
            while (!string.IsNullOrEmpty(perfomanse) && perfomanse != "Нет доступных заданий. Заходите позже!")
            {
                ButtonsVisible();
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                var titleTask = GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[2]/p").GetInnerText();

                SwitchToLastTab();
                if (!_vkManager.IsPostFound())
                {
                    SkipTask("vkLike");
                    perfomanse = GetElementByClassName("groups").GetInnerText();
                    continue;
                }

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
                Thread.Sleep(2000);

                if (IsMaxLikes())
                {
                    OpenPageInNewTab(url);
                    _vkManager.RemoveLike();
                    SkipTask("vkLike");

                    perfomanse = GetElementByClassName("groups").GetInnerText();
                    continue;
                }
                else if (GetTextFromAlert() == "Лайк не был поставлен")
                {
                    if (DelayPayments())
                    {
                        OpenPageInNewTab(url);
                        _vkManager.RemoveLike();
                        CloseTab();
                        SwitchToTab();

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
        /// Подписки в инстаграмм
        /// </summary>
        protected void SubscriptionsInInstagram()
        {
            GetElementById("in0").Click();

            var groups = GetElementsByClassName("groups");
            while (groups.Count() != 0)
            {
                _taskId = groups.First().FindElement(SearchMethod.ClassName, "group").GetId();

                GetElementByXPath("//*[@id='content']/div[2]/div[1]/div[3]/a").Click();
                SwitchToLastTab();
                Thread.Sleep(2000);

                if (_instaManager.IsFoundPage())
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
            GetElementById("in1").Click();

            var groups = GetElementsByClassName("groups");
            while (groups.Count() != 0)
            {
                _taskId = groups.First().FindElement(SearchMethod.ClassName, "group").GetId();

                GetElementByXPath("//*[@id='content']/div[2]/div[1]/div[3]/a").Click();
                SwitchToLastTab();
                Thread.Sleep(2500);

                if (_instaManager.IsFoundPage())
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
        /// Задержка платежа
        /// </summary>
        public bool DelayPayments() //TODO
        {
            try
            {
                int counter = 0;
                var modal = GetElementById("modal");
                while (modal.Displayed)
                {
                    GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                    Thread.Sleep(1500);

                    while (true)
                    {
                        string text = GetTextFromAlert();
                        if (!string.IsNullOrWhiteSpace(text) || !modal.Displayed)
                        {
                            break;
                        }
                        //TODO:Еще может быть такое условие: Список участников скрыт, проверить выполнение нет возможности. Пожалуйста, пропустите это задание.
                    }

                    AlertAccept();

                    counter++;
                    if (counter == 10)
                    {
                        return true;
                    }

                    Thread.Sleep(2000);
                }
            }
            catch
            {
                //Alert'a нет, значит модал. окно закрылось
            }

            Thread.Sleep(3000);
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
            Thread.Sleep(1500);

            switch (typeTask)
            {
                case "vkLike":
                    GetElementByXPath("//*[@id='content']/div[3]/div/div[3]/span").Click();
                    AlertAccept();
                    break;
                case "instaSubscription":
                case "instaLike":
                    string taskId = GetElementsByClassName("groups").First()
                        .FindElement(SearchMethod.ClassName, "group").GetId();
                    if (_taskId == taskId)
                    {
                        GetElementByXPath("//*[@id='content']/div[2]/div/a[1]").Click();
                        AlertAccept();
                    }
                    break;
                case "vkCommunity":
                    GetElementByXPath("//*[@id='content']/div[3]/div/a[1]").Click();
                    break;
            }

            _taskId = string.Empty;
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


        /// <summary>
        /// Авторизация
        /// </summary>
        protected void AuthorizationOnService()
        {
            var login = GetElementByXPath("//*[@id='uLogin']/div");
            if (login != null)
            {
                login.Click();
                Thread.Sleep(1500);
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
