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
using System.Collections.Generic;
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
        /// Дата и время засыпания
        /// </summary>
        private static DateTime? _dateAndTimeFallingAsleep;
        private int _countExceptions;
        /// <summary>
        /// Идентификатор активной задачи
        /// </summary>
        private string _activeTaskId;

        /// <summary>
        /// Тип социальной сети
        /// </summary>
        protected enum SocialNetworkType
        {
            /// <summary>
            /// Инстаграм
            /// </summary>
            Instagram = 0,
            /// <summary>
            /// Вконтакте
            /// </summary>
            Vkontakte = 1
        }

        private IVkManager _vkManager;
        private IInstagramManager _instagramManager;
        private ILogManager _logManager;

        protected void Init()
        {
            Initialization(BROWSER_PROFILE_SERVICE);
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
            _instagramManager = new InstagramManager();
            _logManager = new LogManager();

            var driver = GetDriver();
            _vkManager.SetContextBrowserManager(driver);
            _instagramManager.SetContextBrowserManager(driver);
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
                _instagramManager.Authorization(accountInstagram.Login, accountInstagram.Password);
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

            try
            {
                AuthorizationOnMainService();

                if (IsTimeToSleep())
                {
                    Quit(Status.InSleeping);
                    return;
                }

                BeginCollecting();
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
        protected void BeginCollecting()
        {
            //int checksCount = 0;

            while (true)
            {
                ExecuteTasksInInstagram();
                ExecuteTasksInVkontakte();

                //if (checksCount == 20)
                //{
                //    Inaction();
                //    checksCount = 0;
                //}
                //else
                //{
                //    checksCount++;
                //}
            }
        }

        /// <summary>
        /// Выполнить задачу в Instagram
        /// </summary>
        protected void ExecuteTasksInInstagram()
        {
            GoToUrl("https://v-like.ru/ru/tasks/in");

            while (true)
            {
                var activeTaskText = GetActiveTask();
                switch (activeTaskText)
                {
                    case "NoTask":
                        return;
                    case "Подписаться на":
                        {
                            var isError = ExecuteTask("Подписаться на", SocialNetworkType.Instagram);
                            if (isError)
                            {
                                continue;
                            }
                        }
                        break;
                    case "Поставить лайк на пост":
                        {
                            var isError = ExecuteTask("Поставить лайк на пост", SocialNetworkType.Instagram);
                            if (isError)
                            {
                                continue;
                            }
                        }
                        break;
                    default:
                        {
                            var isError = ExecuteTask(activeTaskText, SocialNetworkType.Instagram);
                            if (isError)
                            {
                                continue;
                            }
                        }
                        break;
                }

                string urlPageToTask = GetUrlPage();
                CloseCurrentTabAndSwitchToAnother();

                var paymentReceived = GetPaidForCompletedTask();
                if (!paymentReceived)
                {
                    UndoTask(activeTaskText, urlPageToTask);
                    RefreshPage();
                    continue;
                }

                RefreshPage();
            }
        }
        /// <summary>
        /// Выполнить задачу в Vkontakte
        /// </summary>
        protected void ExecuteTasksInVkontakte()
        {
            GoToUrl("https://v-like.ru/ru/tasks/vk");

            while (true)
            {
                var activeTaskText = GetActiveTask();
                switch (activeTaskText)
                {
                    case "NoTask":
                        return;
                    case "Подписаться на сообщество":
                        {
                            var isError = ExecuteTask("Подписаться на сообщество", SocialNetworkType.Vkontakte);
                            if (isError)
                            {
                                continue;
                            }
                        }
                        break;
                    case "Поставить лайк и репост":
                        {
                            var isError = ExecuteTask("Поставить лайк и репост", SocialNetworkType.Vkontakte);
                            if (isError)
                            {
                                continue;
                            }
                        }
                        break;
                    default:
                        {
                            var isError = ExecuteTask(activeTaskText, SocialNetworkType.Vkontakte);
                            if (isError)
                            {
                                continue;
                            }
                        }
                        break;
                }

                string urlPageToTask = GetUrlPage();
                CloseCurrentTabAndSwitchToAnother();

                var paymentReceived = GetPaidForCompletedTask();
                if (!paymentReceived)
                {
                    UndoTask(activeTaskText, urlPageToTask);
                    RefreshPage();
                    continue;
                }

                RefreshPage();
            }
        }

        /// <summary>
        /// Выполнить задачу
        /// </summary>
        /// <param name="activeTaskText">Текст активной задачи</param>
        /// <param name="socialNetworkType">Тип социальной сети</param>
        /// <returns>True - Задача не выполнена, иначе false</returns>
        protected bool ExecuteTask(string activeTaskText, SocialNetworkType socialNetworkType)
        {
            SwitchToLastTab();

            bool isError = false;

            switch (activeTaskText)
            {
                case "Подписаться на":
                    {
                        if (_instagramManager.IsFoundPage())
                        {
                            isError = true;
                            break;
                        }

                        _instagramManager.Subscribe();
                        break;
                    }
                case "Поставить лайк на пост":
                    {
                        if (_instagramManager.IsFoundPage())
                        {
                            isError = true;
                            break;
                        }

                        _instagramManager.PutLike();
                        break;
                    }
                case "Подписаться на сообщество":
                    {
                        if (_vkManager.IsPrivateGroup() || _vkManager.IsBlockedCommunity())
                        {
                            isError = true;
                            break;
                        }

                        _vkManager.JoinToComunity();
                        break;
                    }
                case "Поставить лайк и репост":
                    {
                        if (!_vkManager.IsPostFound())
                        {
                            isError = true;
                            break;
                        }

                        _vkManager.PutLike();
                        _vkManager.MakeRepost();
                        break;
                    }
                default:
                    {
                        isError = true;

                        if (socialNetworkType == SocialNetworkType.Instagram)
                        {
                            _logManager.SendToEmail(activeTaskText, "ExecuteTasksInInstagram()", GetUrlPage(), "Новая задача для сервиса V-Like");
                            break;
                        }

                        _logManager.SendToEmail(activeTaskText, "ExecuteTasksInVkontakte()", GetUrlPage(), "Новая задача для сервиса V-Like");
                    }
                    break;
            }

            if (isError)
            {
                CloseCurrentTabAndSwitchToAnother();
                SkipActiveTask();
                RefreshPage();
            }

            return isError;
        }
        /// <summary>
        /// Отменить задачу
        /// </summary>
        protected void UndoTask(string activeTaskText, string urlPageToTask)
        {
            GoToUrl(urlPageToTask);
            OpenPageInNewTabAndSwitch(urlPageToTask);

            switch (activeTaskText)
            {
                case "Подписаться на":
                    if (_instagramManager.IsFoundPage())
                    {
                        break;
                    }

                    _instagramManager.Unsubscribe();
                    break;
                case "Поставить лайк на пост":
                    if (_instagramManager.IsFoundPage())
                    {
                        break;
                    }

                    _instagramManager.RemoveLike();
                    break;
            }

            CloseCurrentTabAndSwitchToAnother();
        }
        /// <summary>
        /// Получить активную задачу
        /// </summary>
        /// <returns>Активная задача для выполнения</returns>
        protected string GetActiveTask()
        {
            string getTaskDetailsScript = "var task = document.querySelector('.list-item.tasks');" +
                                          "if (task === null)" +
                                          "{" +
                                             "return 'NoTask';" +
                                          "}" +
                                          "var taskId = task.id;" +
                                          "var taskDescription = task.getElementsByTagName('a')[0].innerText;" +
                                          "task.getElementsByTagName('a')[2].click();" +
                                          "return taskId + '|' + taskDescription;";

            var taskDetails = ExecuteScript(getTaskDetailsScript).Split("|").ToList();
            if (taskDetails.Count == 2)
            {
                _activeTaskId = taskDetails.First();
                return GetTextTask(taskDetails.Last());
            }

            return GetTextTask(taskDetails.First());
        }
        /// <summary>
        /// Получить текст задачи
        /// </summary>
        /// <param name="task">Задача</param>
        /// <returns>Типизированный текст задачи</returns>
        protected string GetTextTask(string task)
        {
            if (task == "NoWork")
            {
                return "NoWork";
            }
            else if (task.Contains("Подписаться на сообщество"))
            {
                return "Подписаться на сообщество";
            }
            else if (task.Contains("Поставить лайк и репост"))
            {
                return "Поставить лайк и репост";
            }
            else if (task.Contains("Подписаться на"))
            {
                return "Подписаться на";
            }
            else
            {
                return task;
            }
        }
        /// <summary>
        /// Пропустить активную задачу
        /// </summary>
        protected void SkipActiveTask()
        {
            ExecuteScript($"document.getElementById('{_activeTaskId}').getElementsByTagName('a')[3].click();");
        }
        /// <summary>
        /// Получить деньги за выполненную задачу
        /// </summary>
        /// <returns>True - деньги получены, иначе false</returns>
        protected bool GetPaidForCompletedTask()
        {
            var getPaidScript = $"document.getElementById('{_activeTaskId}').getElementsByTagName('a')[1].click();" +
                                "setInterval(function () " +
                                "{" +
                                    "var alertError = document.querySelector('.animated.fadeInDown');" +
                                    $"var buttonText = document.getElementById('{_activeTaskId}').getElementsByTagName('a')[1].innerText;" +
                                    "if (buttonText === 'задание выполнено')" +
                                    "{" +
                                        "return buttonText;" +
                                    "}" +
                                    "else if (alertError != null)" +
                                    "{" +
                                        "return alertError.getElementsByTagName('span')[2].innerText;" +
                                    "}" +
                                "}, 2000)";

            int checksCount = 0;
            while (true)
            {
                var scriptResultText = ExecuteScript(getPaidScript);
                switch (scriptResultText)
                {
                    case "задание выполнено":
                        return true;
                    case "Убедитесь, что вы подписались на страницу, и попробуйте еще раз.":
                        {
                            if (checksCount == 3)
                            {
                                return false;
                            }

                            checksCount++;
                        }
                        break;
                    default:
                        _logManager.SendToEmail(scriptResultText, "GetPaidForCompletedTask()", GetUrlPage(), "Непредвиденный кейс");
                        return false;
                }
            }
        }


        /// <summary>
        /// Бездействовать
        /// </summary>
        protected void Inaction()
        {
            GoToUrl("chrome://newtab");

            int randomTimerMinuts = GetRandomNumber(10, 31);
            while (true)
            {
                if (randomTimerMinuts == 0)
                {
                    GoToUrl("https://v-like.ru/");
                    return;
                }

                Thread.Sleep(60000);
                randomTimerMinuts--;
            }
        }
        /// <summary>
        /// Авторизация на основном сервисе
        /// </summary>
        protected void AuthorizationOnMainService()
        {
            var loginButton = GetElementByClassName("btn-login");
            if (loginButton == null)
            {
                return;
            }

            loginButton.ToClick(2000);

            GetElementByCssSelector(".btn-oauth.vk").ToClick(2000);
            CloseTabByIndex(1);
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
            internetService.BalanceOnService = GetElementById("sidebar_balance").GetInnerText();
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
