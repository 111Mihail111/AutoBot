using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.Managers.Interface;
using AutoBot.Area.Models;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Extentions;
using AutoBot.Models;
using Microsoft.CodeAnalysis;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class VLike : BrowserManager, IVLike
    {
        const string BROWSER_PROFILE_SERVICE = "C:\\_AutoBot\\Profiles\\PerformanceTasks\\V_Like\\";

        /// <summary>
        /// Была ли авторизация соц. сетей
        /// </summary>
        private static bool _isAuthorizationSocialNetworks;
        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        private string _taskId;
        /// <summary>
        /// Дата и время засыпания
        /// </summary>
        private static DateTime? _dateAndTimeFallingAsleep;
        private int _countExceptions;

        private IVkManager _vkManager;
        private IInstagramManager _instaManager;
        private ILogManager _logManager;

        protected void Init()
        {
            Initialization(BROWSER_PROFILE_SERVICE, true);
            SetContextForManagers();

            if (!_isAuthorizationSocialNetworks)
            {
                AuthorizationSocialNetworks();
            }

            SetSleepTimer();
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

            _isAuthorizationSocialNetworks = true;
        }
        /// <summary>
        /// Установить таймер сна
        /// </summary>
        protected void SetSleepTimer()
        {
            if (_dateAndTimeFallingAsleep != null)
            {
                return;
            }

            _dateAndTimeFallingAsleep = DateTime.Now.AddHours(13);
        }


        public void GoTo(string url)
        {
            Init();
            GoToUrl(url);
            AuthorizationOnService();

            try
            {
                if (IsTimeToSleep())
                {
                    Quit(Status.InSleeping);
                    return;
                }

                BeginCollecting();
                Quit(Status.Work);
            }
            catch (Exception exception)
            {
                _countExceptions++;
                _logManager.SendToEmail(GenerateMessage(exception, "Произошла ошибка"));

                Quit(Status.Work);
            }
            finally
            {
                if (_countExceptions == 50)
                {
                    _logManager.SendToEmail("TODO:Здесь будут все возникшие ошибки за день", "Колличество ошибок за сегодня достигло своего предела.");
                    Quit(Status.NoWork);
                }
            }
        }
        /// <summary>
        /// Начать сбор
        /// </summary>
        private void BeginCollecting()
        {
            JoinVkCommunity();
            WorkWithLikesAndRepostVK();
            SubscriptionsInInstagram();
            WorkWithLikeInstagram();
        }


        /// <summary>
        /// Вступить в сообщество ВК
        /// </summary>
        protected void JoinVkCommunity()
        {
            GetElementById("vk1").ToClick();

            var message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            while (message == null)
            {
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").ToClick();
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

                if (!DidPaymentPass())
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
            GetElementById("vk2").ToClick();

            var perfomanse = GetElementByClassName("groups")?.GetInnerText();
            while (!string.IsNullOrEmpty(perfomanse) && perfomanse != "Нет доступных заданий. Заходите позже!")
            {
                ButtonsVisible();
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").ToClick();
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

                string url = GetUrlPage();
                _vkManager.PutLike();
                
                CloseTab();
                SwitchToTab();

                if (!DidPaymentPass())
                {
                    OpenPageInNewTab(url);
                    _vkManager.RemoveLike();
                    SkipTask("vkLike");
                }

                perfomanse = GetElementByClassName("groups").GetInnerText();
            }
        }
        /// <summary>
        /// Подписки в инстаграмм
        /// </summary>
        protected void SubscriptionsInInstagram()
        {
            GetElementById("in0").ToClick();

            var groups = GetElementsByClassName("groups");
            while (groups.Count() != 0)
            {
                _taskId = groups.First().FindElement(SearchMethod.ClassName, "group").GetId();

                GetElementByXPath("//*[@id='content']/div[2]/div[1]/div[3]/a").ToClick();
                SwitchToLastTab();

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

                if (!DidPaymentPass())
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
            GetElementById("in1").ToClick();

            var groups = GetElementsByClassName("groups");
            while (groups.Count() != 0)
            {
                _taskId = groups.First().FindElement(SearchMethod.ClassName, "group").GetId();

                GetElementByXPath("//*[@id='content']/div[2]/div[1]/div[3]/a").ToClick();
                SwitchToLastTab();

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

                if (!DidPaymentPass())
                {
                    OpenPageInNewTab(url);
                    _instaManager.RemoveLike();
                    SkipTask("instaLike");
                }

                groups = GetElementsByClassName("groups");
            }
        }


        /// <summary>
        /// Прошел ли платеж
        /// </summary>
        /// <returns>True - прошел, иначе false</returns>
        public bool DidPaymentPass()
        {
            int counter = 0;
            var paymentButton = GetPaymentButton();

            var modal = GetElementById("modal");
            while (modal.Displayed)
            {
                if (counter == 7)
                {
                    return false;
                }

                paymentButton.ToClick();

                if (!IsAlertExist(5))
                {
                    return true;
                }

                string text = GetTextFromAlert();
                switch (text)
                {
                    case "К сожалению, уже было поставлено нужное количество лайков к данному объекту. Обновите список заданий.":
                    case "Список участников скрыт, проверить выполнение нет возможности.Пожалуйста, пропустите это задание.":
                        AlertAccept();
                        return false;
                    default:
                        counter = 6;
                        _logManager.SendToEmail(text, "DidPaymentPass()", string.Empty, "Непредвиденный кейс в alert-окне");
                        break;
                }

                AlertAccept();
                counter++;
            }

            return true;
        }
        /// <summary>
        /// Пропустить задание
        /// </summary>
        public void SkipTask(string typeTask)
        {
            CloseTab();
            SwitchToTab();
            GetElementByXPath("//*[@id='buttons']/a[1]").ToClick(1500);

            switch (typeTask)
            {
                case "vkLike":
                    GetElementByXPath("//*[@id='content']/div[3]/div/div[3]/span").ToClick();
                    AlertAccept();
                    break;
                case "instaSubscription":
                case "instaLike":
                    string taskId = GetElementsByClassName("groups").First().FindElement(SearchMethod.ClassName, "group").GetId();
                    if (_taskId == taskId)
                    {
                        GetElementByXPath("//*[@id='content']/div[2]/div/a[1]").ToClick();
                        AlertAccept();
                    }
                    break;
                case "vkCommunity":
                    GetElementByXPath("//*[@id='content']/div[3]/div/a[1]").ToClick();
                    break;
            }

            _taskId = string.Empty;
            Thread.Sleep(2000);
        }
        /// <summary>
        /// Получить кнопку оплаты
        /// </summary>
        /// <returns>Кнопка готовая к нажатию</returns>
        public IWebElement GetPaymentButton()
        {
            var button = GetElementByXPath("//*[@id='buttons']/a[2]");
            while (!button.Displayed)
            {
                Thread.Sleep(1000);
                button = GetElementByXPath("//*[@id='buttons']/a[2]");
            }

            return button;
        }


        /// <summary>
        /// Авторизация
        /// </summary>
        protected void AuthorizationOnService()
        {
            var login = GetElementByXPath("//*[@id='uLogin']/div");
            if (login != null)
            {
                login.ToClick(1500);
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
        /// Пришло ли время засыпать
        /// </summary>
        /// <returns>True - пришло, иначе false</returns>
        protected bool IsTimeToSleep()
        {
            return DateTime.Now >= _dateAndTimeFallingAsleep;
        }
        /// <summary>
        /// Получить случайное число
        /// </summary>
        /// <param name="minValue">Минимальное значение</param>
        /// <param name="maxValue">Максимальное значение</param>
        /// <returns>Случайное число</returns>
        protected int GetRandomNumber(int minValue, int maxValue)
        {
            return new Random().Next(minValue, maxValue);
        }
        /// <summary>
        /// Закрыть браузер
        /// </summary>
        /// <param name="status">Статус сервиса</param>
        public void Quit(Status status)
        {
            UpdateModel(status);
            QuitBrowser();
        }
        /// <summary>
        /// Получить детали интернет-сервиса
        /// </summary>
        /// <param name="internetService">Интернет-сервис</param>
        /// <returns>Модель интернет-сервиса</returns>
        protected void UpdateModel(Status status)
        {
            var internetService = WebService.GetInternetServices().Where(w => w.TypeService == TypeService.VLike).FirstOrDefault();
            internetService.ActivationTime = TimeSpan.FromMinutes(1);
            internetService.BalanceOnService = GetElementByClassName("balance").GetInnerText();
            internetService.StatusService = status;

            switch (status)
            {
                case Status.InSleeping:
                    _dateAndTimeFallingAsleep = null;
                    _countExceptions = 0;
                    internetService.LaunchTime = DateTime.Now.AddHours(11).AddMinutes(2 * GetRandomNumber(1, 4));
                    break;
                case Status.NoWork:
                    _countExceptions = 0;
                    break;
            }

            WebService.UpdateInternetService(internetService);
        }
        /// <summary>
        /// Сформировать сообщение
        /// </summary>
        /// <param name="exception">Исключение</param>
        /// <param name="topic">Тема</param>
        /// <returns>Сообщение</returns>
        protected Message GenerateMessage(Exception exception, string topic)
        {
            return new Message
            {
                Url = GetUrlPage(),
                Exception = exception,
                Base64Encoded = GetScreenshot().AsBase64EncodedString,
                TabsCount = GetTabsCount(),
                Topic = topic,
            };
        }
    }
}
