﻿using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.Managers.Interface;
using AutoBot.Area.Models;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Extentions;
using AutoBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class VkTarget : BrowserManager, IVkTarget
    {
        const string BROWSER_PROFILE_SERVICE = "C:\\_AutoBot\\Profiles\\PerformanceTasks\\";

        /// <summary>
        /// Была ли авторизация соц. сетей
        /// </summary>
        private static bool _isAuthorizationSocialNetworks;
        /// <summary>
        /// Тип соц. сети
        /// </summary>
        private string _typeSocialNetwork;
        /// <summary>
        /// Задача
        /// </summary>
        private string _task;
        /// <summary>
        /// Идентификатор задачи
        /// </summary>
        private int _taksId;
        /// <summary>
        /// Url-страницы
        /// </summary>
        private string _urlByTask;
        /// <summary>
        /// Количество действий
        /// </summary>
        private int _countAction;
        /// <summary>
        /// Количество исключений
        /// </summary>
        private int _countExceptions = 0;

        /// <summary>
        /// Дата и время засыпания
        /// </summary>
        private DateTime? _dateAndTimeFallingAsleep;
        /// <summary>
        /// Тип сервиса
        /// </summary>
        private TypeService _typeService;

        private IVkManager _vkManager;
        private IClassmatesManager _classmatesManager;
        private IYandexZenManager _yandexZenManager;
        private ITumblr _tumblrManager;
        private IReddit _redditManager;
        private IQuora _quoraManager;
        private IVimeoManager _vimeoManager;
        private ILogManager _logManager;

        protected void Init()
        {
            Initialization($"{BROWSER_PROFILE_SERVICE + _typeService}\\");
            SetContextForManagers();
            SetSleepTimer();

            if (!_isAuthorizationSocialNetworks)
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
            _classmatesManager = new ClassmatesManager();
            _yandexZenManager = new YandexZenManager();
            _tumblrManager = new TumblrManager();
            _redditManager = new RedditManager();
            _quoraManager = new QuoraManager();
            _vimeoManager = new VimeoManager();
            _logManager = new LogManager();

            var driver = GetDriver();
            _vkManager.SetContextBrowserManager(driver);
            _classmatesManager.SetContextBrowserManager(driver);
            _yandexZenManager.SetContextBrowserManager(driver);
            _tumblrManager.SetContextBrowserManager(driver);
            _redditManager.SetContextBrowserManager(driver);
            _quoraManager.SetContextBrowserManager(driver);
            _vimeoManager.SetContextBrowserManager(driver);
        }
        /// <summary>
        /// Авторизация в соц сетях
        /// </summary>
        protected void AuthorizationSocialNetworks()
        {
            var accounts = AccountService.GetAccountsByType(_typeService);

            var accountVK = accounts.Where(w => w.AccountType == AccountType.Vk).FirstOrDefault();
            if (accountVK != null)
            {
                _vkManager.Authorization(accountVK.Login, accountVK.Password);
                CloseCurrentTabAndSwitchToAnother();
            }

            var accountReddit = accounts.Where(w => w.AccountType == AccountType.Reddit).FirstOrDefault();
            if (accountReddit != null)
            {
                _redditManager.Authorization(accountReddit.Login, accountReddit.Password);
            }

            var accountClassmates = accounts.Where(w => w.AccountType == AccountType.Classmates).FirstOrDefault();
            if (accountClassmates != null)
            {
                _classmatesManager.AuthorizationThroughMail(accountClassmates.Login, accountClassmates.Password);
            }

            var accountYandexZen = accounts.Where(w => w.AccountType == AccountType.YandexZen).FirstOrDefault();
            if (accountYandexZen != null)
            {
                _yandexZenManager.Authorization(accountYandexZen.Login, accountYandexZen.Password);
                CloseCurrentTabAndSwitchToAnother();
            }

            var accountTumblr = accounts.Where(w => w.AccountType == AccountType.Tumblr).FirstOrDefault();
            if (accountTumblr != null)
            {
                _tumblrManager.Authorization(accountTumblr.Login, accountTumblr.Password);
            }

            var accountQuora = accounts.Where(w => w.AccountType == AccountType.Quora).FirstOrDefault();
            if (accountQuora != null)
            {
                _quoraManager.Authorization(accountQuora.Login, accountQuora.Password);
            }

            var accountVimeo = accounts.Where(w => w.AccountType == AccountType.Vimeo).FirstOrDefault();
            if (accountVimeo != null)
            {
                _vimeoManager.Authorization(accountVimeo.Login, accountVimeo.Password);
                CloseCurrentTabAndSwitchToAnother();
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


        public void GoTo(string url, TypeService typeService)
        {
            _typeService = typeService;

            Init();
            GoToUrl(url);

            bool isWork = true;
            while (isWork)
            {
                try
                {
                    AuthorizationOnService();

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

                    isWork = ProcessingException(exception);
                }
                finally
                {
                    if (_countExceptions == 50)
                    {
                        _logManager.SendToEmail("TODO:Здесь будут все возникшие ошибки за день", "Колличество ошибок за сегодня достигло своего предела.");
                        Quit(Status.NoWork);
                        isWork = false;
                    }
                }
            }
        }
        /// <summary>
        /// Обработать исключение
        /// </summary>
        /// <param name="exception">Исключение</param>
        /// <returns>True - ошибка обработана, иначе false</returns>
        protected bool ProcessingException(Exception exception) //Есть TODO
        {
            try
            {
                int tabsCount = GetTabsCount();
                switch (tabsCount)
                {
                    case 1:
                        _logManager.SendToEmail(GetMessage(exception, "Ошибка на головном сайте. Работа с ним прекращена."));
                        Quit(Status.NoWork);

                        return false;
                    case 2:
                        {
                            _logManager.SendToEmail(GetMessage(exception, "Ошибка при выполнении задания."));

                            CloseCurrentTabAndSwitchToAnother();
                            GoToUrl("https://vktarget.ru/");
                            SkipTask();

                            return true;
                        }
                    default:
                        _logManager.SendToEmail(GetMessage(exception, "Неопределенная ошибка. Количество вкладок превысило 2."));
                        return false;
                }

                //TODO:Проверка авторизации
                //TODO:Проверка работоспособности страницы
            }
            catch (Exception newException)
            {
                _logManager.SendToEmail(new Message
                {
                    Topic = $"Ошибка в {_typeService} при обработке исключения в ProcessingException()",
                    Exception = newException
                });

                //Quit(Status.NoWork); //TODO: Нужно как-то проверять, что у нас закрыт браузер. Если мы будем закрывать то, что и так закрыто, то получим ошибку.

                return false;
            }
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
        /// Начать сбор
        /// </summary>
        protected void BeginCollecting()
        {
            GetTask();
            switch (_typeSocialNetwork)
            {
                case "vk":
                    CarryOutTaskInVk(_task);
                    break;
                case "odnoklassniki":
                    CarryOutTaskInСlassmates(_task);
                    break;
                case "zen":
                    CarryOutTaskInZen(_task);
                    break;
                case "reddit":
                    CarryOutTaskInReddit(_task);
                    break;
                case "tumblr":
                    CarryOutTaskInTumblr(_task);
                    break;
                case "quora":
                    CarryOutTaskInQuora(_task);
                    break;
                case "vimeo":
                    CarryOutTaskInVimeo(_task);
                    break;
                case "NoTasks":
                    ShowActivity();
                    break;
                default:
                    _logManager.SendToEmail(_typeSocialNetwork, "BeginCollecting()", GetUrlPage(), "Новая соц. сеть в выдаче");
                    SkipTask();
                    break;
            }

            _typeSocialNetwork = string.Empty;
            _task = string.Empty;
            _urlByTask = string.Empty;
            _taksId = 0;

            UpdateModel(Status.InWork);
        }


        /// <summary>
        /// Выполнить задачу в VK
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInVk(string taskText) //есть TODO
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            bool isError = false;
            switch (taskText)
            {
                case "Вступите в сообщество":
                    {
                        if (_vkManager.IsBlockedCommunity())
                        {
                            isError = true;
                            break;
                        }

                        _vkManager.JoinToComunity();
                        break;
                    }
                case "Поставьте лайк на странице":
                    {
                        if (!_vkManager.IsPostFound())
                        {
                            isError = true;
                            break;
                        }

                        _vkManager.PutLike();
                        break;
                    }
                case "Посмотреть пост":
                    Thread.Sleep(500);
                    break;
                case "Нажмите поделиться записью": //https://vk.com/wall-203080055_11
                    {
                        if (!_vkManager.IsPostFound())
                        {
                            isError = true;
                            break;
                        }

                        _vkManager.MakeRepost();
                        break;
                    }
                case "Добавить в друзья":
                    {
                        if (_vkManager.IsBlockedAccount())
                        {
                            isError = true;
                            break;
                        }

                        _vkManager.AddToFriends();
                        break;
                    }
                case "Расскажите о группе":
                    {
                        if (_vkManager.IsBlockedCommunity())
                        {
                            isError = true;
                            break;
                        }

                        _vkManager.ToTellAboutGroup();
                        break;
                    }
                default:
                    _logManager.SendToEmail(taskText, "CarryOutTaskInVk()", GetUrlPage(), "Новая задача");
                    isError = true;
                    break;
            }

            CloseCurrentTabAndSwitchToAnother();

            if (isError)
            {
                SkipTask();
                return;
            }

            GetPayout();
        }
        /// <summary>
        /// Выполнить задачу в Сlassmates
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInСlassmates(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            bool isError = false;
            switch (taskText)
            {
                case "Вступить в группу":
                    _classmatesManager.JoinGroup();
                    break;
                case "Поставьте класс под записью":
                    {
                        if (_classmatesManager.IsBlokedContent())
                        {
                            isError = true;
                            break;
                        }

                        _classmatesManager.PutClass();
                        break;
                    }
                case "Поставить 'Класс' на публикации":
                    _classmatesManager.PutClass();
                    break;
                case "Поделиться записью":
                    {
                        if (_classmatesManager.IsBlokedContent())
                        {
                            isError = true;
                            break;
                        }

                        _classmatesManager.MakeRepost();
                        break;
                    }
                case "Добавить в друзья":
                    _classmatesManager.AddToFriends();
                    break;
                default:
                    _logManager.SendToEmail(taskText, "CarryOutTaskInСlassmates()", GetUrlPage(), "Новая задача");
                    isError = true;
                    break;
            }

            CloseCurrentTabAndSwitchToAnother();

            if (isError)
            {
                SkipTask();
                return;
            }

            GetPayout();
        }
        /// <summary>
        /// Выполнить задачу в Yandex.Zen
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInZen(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            bool isError = false;
            switch (taskText)
            {
                case "Поставьте лайк на пост":
                    _yandexZenManager.PutLike();
                    break;
                case "Подпишитесь на пользователя":
                    _yandexZenManager.Subscribe();
                    break;
                default:
                    _logManager.SendToEmail(taskText, "CarryOutTaskInZen()", GetUrlPage(), "Новая задача");
                    isError = true;
                    break;
            }

            CloseTab();
            SwitchToTab();

            if (isError)
            {
                SkipTask();
                return;
            }

            GetPayout();
        }
        /// <summary>
        /// Выполнить задачу в Reddit
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInReddit(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            bool isError = false;
            switch (taskText)
            {
                case "Нажать стрелку вверх на Запись":
                    try
                    {
                        _redditManager.UpArrowForPost();
                    }
                    catch (Exception exception)
                    {
                        isError = true;
                        _logManager.SendToEmail(GetMessage(exception, "Возникла ожидаемая ошибка"));
                    }
                    break;
                case "Подпишитесь на пользователя":
                    var panel = GetElementByClassName("bDDEX4BSkswHAG_45VkFB");
                    if (panel != null)
                    {
                        panel.FindElement(SearchMethod.Tag, "button").ToClick(2500);
                    }
                    _redditManager.Subscribe();
                    break;
                default:
                    _logManager.SendToEmail(taskText, "CarryOutTaskInReddit()", GetUrlPage(), "Новая задача");
                    isError = true;
                    break;
            }

            CloseTab();
            SwitchToTab();

            if (isError)
            {
                SkipTask();
                return;
            }

            GetPayout();
        }
        /// <summary>
        /// Выполнить задачу в Tumblr
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInTumblr(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            bool isError = false;
            switch (taskText)
            {
                case "Реблогните блог":
                    _tumblrManager.Reblog();
                    break;
                case "Поставить лайк на пост":
                    _tumblrManager.LikePost();
                    break;
                case "Подпишитесь на блог":
                case "подпишитесь на блог":
                    _tumblrManager.SubscribeToBlog();
                    break;
                default:
                    _logManager.SendToEmail(taskText, "CarryOutTaskInTumblr()", GetUrlPage(), "Новая задача");
                    isError = true;
                    break;
            }

            CloseTab();
            SwitchToTab();

            if (isError)
            {
                SkipTask();
                return;
            }

            GetPayout();
        }
        /// <summary>
        /// Выполнить задачу в Quora
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInQuora(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            bool isError = false;
            switch (taskText)
            {
                case "Подписаться на пользователя":
                    if (!_quoraManager.IsUserPageProfile())
                    {
                        isError = true;
                        break;
                    }
                    _quoraManager.Subscribe();
                    break;
                case "Поставьте лайк на ответ":
                    _quoraManager.LikeAnswer();
                    break;
                case "Поделиться ответом":
                    if (!_quoraManager.IsPageFound())
                    {
                        isError = true;
                        break;
                    }
                    _quoraManager.MakeRepost();
                    break;
                default:
                    _logManager.SendToEmail(taskText, "CarryOutTaskInQuora()", GetUrlPage(), "Новая задача");
                    isError = true;
                    break;
            }

            CloseTab();
            SwitchToTab();

            if (isError)
            {
                SkipTask();
                return;
            }

            GetPayout();
        }
        /// <summary>
        /// Выполнить задачу в Vimeo
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInVimeo(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            bool isError = false;
            switch (taskText)
            {
                case "Поставить лайк на видео":
                    _vimeoManager.LikeUnderVideo();
                    break;
                case "подпишитесь на Аккаунт":
                    _vimeoManager.Subscribe();
                    break;
                default:
                    _logManager.SendToEmail(taskText, "CarryOutTaskInVimeo()", GetUrlPage(), "Новая задача");
                    isError = true;
                    break;
            }

            CloseTab();
            SwitchToTab();

            if (isError)
            {
                SkipTask();
                return;
            }

            GetPayout();
        }


        /// <summary>
        /// Отменить задание
        /// </summary>
        protected void UndoTask()
        {
            switch (_typeSocialNetwork)
            {
                case "vk":
                    UndoTaskInVk();
                    break;
                case "odnoklassniki":
                    UndoTaskInСlassmates();
                    break;
                case "zen":
                    UndoTaskInZen();
                    break;
                case "reddit":
                    UndoTaskInReddit();
                    break;
                case "quora":
                    UndoTaskInQuora();
                    break;
                case "tumblr":
                    UndoTaskInTumblr();
                    break;
                case "vimeo":
                    UndoTaskInVimeo();
                    break;
            }
        }
        /// <summary>
        /// Отменить задачу в Vk
        /// </summary>
        protected void UndoTaskInVk()
        {
            OpenPageInNewTabAndSwitch(_urlByTask);

            switch (_task)
            {
                case "Вступите в сообщество":
                    _vkManager.UnsubscribeToComunity();
                    break;
                case "Поставьте лайк на странице":
                    _vkManager.RemoveLike();
                    break;
                case "Добавить в друзья":
                    _vkManager.RemoveFriend();
                    break;
            }

            CloseTab();
            SwitchToTab();
        }
        /// <summary>
        /// Отменить задачу в Сlassmates
        /// </summary>
        protected void UndoTaskInСlassmates()
        {
            OpenPageInNewTabAndSwitch(_urlByTask);

            switch (_task)
            {
                case "Вступить в группу":
                    _classmatesManager.LeaveGroup();
                    break;
                case "Поставьте класс под записью":
                case "Поставить 'Класс' на публикации":
                    _classmatesManager.RemoveClass();
                    break;
                case "Добавить в друзья":
                    _classmatesManager.RemoveToFriends();
                    break;
            }

            CloseTab();
            SwitchToTab();
        }
        /// <summary>
        /// Отменить задачу в Yandex.Zen
        /// </summary>
        protected void UndoTaskInZen()
        {
            OpenPageInNewTabAndSwitch(_urlByTask);

            switch (_task)
            {
                case "Поставьте лайк на пост":
                    _yandexZenManager.RemoveLike();
                    break;
                case "Подпишитесь на пользователя":
                    _yandexZenManager.Unsubscribe();
                    break;
            }

            CloseTab();
            SwitchToTab();
        }
        /// <summary>
        /// Отменить задачу в Reddit
        /// </summary>
        protected void UndoTaskInReddit()
        {
            OpenPageInNewTabAndSwitch(_urlByTask);

            switch (_task)
            {
                case "Нажать стрелку вверх на Запись":
                    _redditManager.DownArrowUnderPost();
                    break;
                case "Подпишитесь на пользователя":
                    _redditManager.Unsubscribe();
                    break;
            }

            CloseTab();
            SwitchToTab();
        }
        /// <summary>
        /// Отменить задачу в Quora
        /// </summary>
        protected void UndoTaskInQuora()
        {
            OpenPageInNewTabAndSwitch(_urlByTask);

            switch (_task)
            {
                case "Подписаться на пользователя":
                    _quoraManager.Unsubscribe();
                    break;
                case "Поставьте лайк на ответ":
                    _quoraManager.RemoveLike();
                    break;
            }

            CloseTab();
            SwitchToTab();
        }
        /// <summary>
        /// Отменить задачу в Tumblr
        /// </summary>
        protected void UndoTaskInTumblr()
        {
            OpenPageInNewTabAndSwitch(_urlByTask);

            switch (_task)
            {
                case "Поставить лайк на пост":
                    _tumblrManager.RemoveLike();
                    break;
                case "Подпишитесь на блог":
                case "подпишитесь на блог":
                    _tumblrManager.UnsubscribeToBlog();
                    break;
            }

            CloseTab();
            SwitchToTab();
        }
        /// <summary>
        /// Отменить задачу в Vimeo
        /// </summary>
        protected void UndoTaskInVimeo()
        {
            OpenPageInNewTabAndSwitch(_urlByTask);

            switch (_task)
            {
                case "Поставить лайк на видео":
                    _vimeoManager.RemoveLike();
                    break;
                case "подпишитесь на Аккаунт":
                    _vimeoManager.Unsubscribe();
                    break;
            }

            CloseTab();
            SwitchToTab();
        }


        /// <summary>
        /// Проявить активность
        /// </summary>
        protected void ShowActivity(bool isRefresh = true)
        {
            _countAction++;

            var randomNumber = GetRandomNumber(0, 3);
            var action = GetAction();

            switch (action)
            {
                case ActionToBrowser.FocusOnElement:
                    FocusOnFirstElementMenu();
                    break;
                case ActionToBrowser.FocusOnElements:
                    FocusOnAllElementsMenu();
                    break;
                case ActionToBrowser.FocusOnTab:
                    SwitchToTab();
                    break;
                //case ActionToBrowser.RefreshPage: На сайте косяк. Пока отрубил эту функцию
                //    {
                //        if (!isRefresh)
                //        {
                //            break;
                //        }
                //        else if (randomNumber == 2)
                //        {
                //            Refresh();
                //            break;
                //        }

                //        FocusOnFirstElementMenu();
                //    }
                //    break;
                case ActionToBrowser.ClickOnElement:
                    {
                        if (randomNumber == 2)
                        {
                            ClickOnElementMenu();
                            break;
                        }

                        FocusOnFirstElementMenu();
                    }
                    break;
                case ActionToBrowser.ClickOnElements:
                    {
                        if (randomNumber == 2)
                        {
                            ClickOnElementsMenu();
                            break;
                        }

                        FocusOnAllElementsMenu();
                    }
                    break;
                case ActionToBrowser.Inaction:
                    {
                        if (_countAction >= 40)
                        {
                            Inaction();
                            _countAction = 0;
                            return;
                        }
                    }
                    break;
            }

            int randomMilliseconds = GetRandomNumber(1000, 5000);
            int randomSleep = GetRandomNumber(2, 4) * randomMilliseconds;

            Thread.Sleep(randomSleep);
        }
        /// <summary>
        /// Пропустить задачу
        /// </summary>
        protected void SkipTask()
        {
            var button = GetElementByCssSelector(".control__item.close");
            if (button.Displayed)
            {
                try
                {
                    button.ToClick();
                }
                catch (Exception exception)
                {
                    _logManager.SendToEmail(GetMessage(exception, "Возникла ожидаемая ошибка"));
                }
            }
        }
        /// <summary>
        /// Получить выплату
        /// </summary>
        protected void GetPayout() //TODO: Переработать метод. Разбить на несколько 
        {
            string getTaskScript = "var task = document.querySelector('.container-fluid.available__table');";
            string clickButtonScript = "task.querySelector('.default__small__btn.check__btn').click();";
            string getAttribute = "return task.children[0].getAttribute('data-task-item');";

            _taksId = Convert.ToInt32(ExecuteScript(getTaskScript + clickButtonScript + getAttribute));
            int waitingСounter = 0;

            while (true)
            {
                var errorPanel = GetElementByClassName("is_error");
                if (errorPanel != null)
                {
                    var dataId = Convert.ToInt32(errorPanel.GetDataId());
                    if (dataId != _taksId)
                    {
                        return;
                    }

                    var error = errorPanel.FindElements(SearchMethod.ClassName, "content").Last().GetInnerText();
                    switch (error)
                    {
                        case "Произошла ошибка":
                        case "Ошибка при выполнении проверки, попробуйте снова":
                        case "Похоже, вы не выполнили задание. Подождите 15 секунд и повторите попытку":
                        case "Проверка не пройдена. Попробуйте через 10 секунд.":
                            {
                                waitingСounter++;
                                if (waitingСounter < 3)
                                {
                                    Thread.Sleep(12000);
                                    ExecuteScript(getTaskScript + clickButtonScript);
                                    continue;
                                }

                                SkipTask();
                                UndoTask();
                                return;
                            }
                        case "Задание уже остановлено. Приносим свои извинения.":
                        case "Увы, но список заданий устарел! Скоро вы получите новые задания.":
                            SkipTask();
                            UndoTask();
                            return;
                        case "Похоже, Вы не выполнили задание":
                        case "Похоже, что вы не поставили класс!":
                            {
                                waitingСounter++;
                                if (waitingСounter < 3)
                                {
                                    Thread.Sleep(3000);
                                    ExecuteScript(getTaskScript + clickButtonScript);
                                    continue;
                                }

                                _logManager.SendToEmail(error, "CheckTask()", _urlByTask, "Задача не прошла проверку");

                                SkipTask();
                                UndoTask();
                                return;
                            }
                        default:
                            _logManager.SendToEmail(error, "CheckTask()", GetUrlPage(), "Непредвиденный кейс");

                            SkipTask();
                            UndoTask();
                            return;
                    }
                }
                else if (_taksId != Convert.ToInt32(ExecuteScript(getTaskScript + getAttribute)))
                {
                    return;
                }
                else
                {
                    if (IsTimeToSleep())
                    {
                        return;
                    }

                    ShowActivity(false);
                }
            }
        }
        /// <summary>
        /// Получить задачу
        /// </summary>
        /// <returns></returns>
        protected void GetTask()
        {
            var taskDetails = ExecuteScript(
                "var taskEmptyLength = document.querySelector('.empty').classList.length;" +
                "if (taskEmptyLength == 1)" +
                "{" +
                    "return 'NoTasks';" +
                "}" +
                "var tasks = document.querySelector('.container-fluid.available__table').getElementsByClassName('row tb__row');" +
                "var systemType = tasks[0].getElementsByClassName('social__img')[0].class.toString().replace('social__img ', '');" +
                "var button = tasks[0].children[2].getElementsByTagName('a')[0];" +
                "var task = document.getElementsByClassName('wrap')[1].innerText;" +
                "button.click();" +
                "return systemType + '|' + task;").Split("|").ToList();

            _typeSocialNetwork = taskDetails.First();

            if (taskDetails.Count == 2)
            {
                _task = taskDetails.Last();
            }
        }
        /// <summary>
        /// Бездействовать
        /// </summary>
        protected void Inaction()
        {
            GoToUrl("chrome://newtab");

            int randomTimerMinuts = GetRandomNumber(10, 61);
            while (true)
            {
                if (randomTimerMinuts == 0)
                {
                    GoToUrl("https://vktarget.ru/");
                    return;
                }

                Thread.Sleep(60000);
                randomTimerMinuts--;
            }
        }


        /// <summary>
        /// Авторизация на сервисе
        /// </summary>
        /// <param name="url">Url-адрес сервиса</param>
        protected void AuthorizationOnService()
        {
            var loginBtn = GetElementById("loginBtn");
            if (loginBtn == null)
            {
                return;
            }

            loginBtn.ToClick(1500);

            GetElementById("uloginVkontakte").ToClick();
            WaitingForAuthorization();
        }
        /// <summary>
        /// Ожидание авторизации
        /// </summary>
        protected void WaitingForAuthorization()
        {
            int counter = 0;

            while (true)
            {
                if (counter == 5)
                {
                    return;
                }

                Thread.Sleep(4000);

                var header = GetElementById("header");
                if (header != null)
                {
                    RefreshPage();
                    return;
                }

                counter++;
            }
        }
        /// <summary>
        /// Получить баланс
        /// </summary>
        protected string GetBalance()
        {
            var spanElement = GetElementByXPath("//*[@id='header']/div/div/div[3]/div/div/div[2]/span[2]");
            return spanElement == null ? string.Empty : spanElement.GetInnerText() + "руб";
        }
        /// <summary>
        /// Обновить модель
        /// </summary>
        /// <param name="status">Статус сервиса</param>
        protected void UpdateModel(Status status)
        {
            var internetService = WebService.GetInternetServices().Where(w => w.TypeService == _typeService).FirstOrDefault();
            var balance = GetBalance();

            internetService.BalanceOnService = string.IsNullOrWhiteSpace(balance) ? internetService.BalanceOnService : balance;
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
        /// Фокус на элемент меню
        /// </summary>
        protected void FocusOnFirstElementMenu()
        {
            var liCollection = GetElementByXPath("//*[@id='list']/main/section[1]/div[2]/div/div/div/ul")
                        .FindElements(SearchMethod.Tag, "li").ToList();

            FocusOnElement(liCollection[GetRandomNumber(0, liCollection.Count)]);
        }
        /// <summary>
        /// Нажать на элемент меню
        /// </summary>
        protected void ClickOnElementMenu()
        {
            GetElementByXPath("//*[@id='list']/main/section[1]/div[2]/div/div/div/ul")
                .FindElements(SearchMethod.Tag, "li").First().Click();
        }
        /// <summary>
        /// Наждать на элементы меню
        /// </summary>
        protected void ClickOnElementsMenu()
        {
            var liCollection = GetElementByXPath("//*[@id='list']/main/section[1]/div[2]/div/div/div/ul")
                        .FindElements(SearchMethod.Tag, "li").ToList();

            foreach (var item in liCollection)
            {
                item.ToClick(1500);
            }

            liCollection.First().Click();
        }
        /// <summary>
        /// Фокус на элементы меню
        /// </summary>
        protected void FocusOnAllElementsMenu()
        {
            var liCollection = GetElementByXPath("//*[@id='list']/main/section[1]/div[2]/div/div/div/ul")
                        .FindElements(SearchMethod.Tag, "li").ToList();

            foreach (var item in liCollection)
            {
                FocusOnElement(item);
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// Обновить страницу
        /// </summary>
        protected void Refresh()
        {
            var logotype = GetElementByXPath("//*[@id='header']/div/div/div[1]/div/a");
            FocusOnElement(logotype);

            logotype.ToClick();
        }
        /// <summary>
        /// Получить действие
        /// </summary>
        /// <returns>Действие в браузере</returns>
        protected ActionToBrowser GetAction() => GetRandomNumber(0, 7) switch
        {
            0 => ActionToBrowser.FocusOnElement,
            1 => ActionToBrowser.FocusOnElements,
            2 => ActionToBrowser.FocusOnTab,
            3 => ActionToBrowser.RefreshPage,
            4 => ActionToBrowser.Inaction,
            5 => ActionToBrowser.ClickOnElement,
            6 => ActionToBrowser.ClickOnElements,
            _ => ActionToBrowser.FocusOnElement,
        };
        /// <summary>
        /// Получить сообщение
        /// </summary>
        /// <param name="exception">Исключение</param>
        /// <param name="topic">Тема</param>
        /// <returns>Модель сообщения</returns>
        protected Message GetMessage(Exception exception, string topic)
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