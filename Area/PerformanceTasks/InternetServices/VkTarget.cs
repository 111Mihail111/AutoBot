using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.Managers.Interface;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Extentions;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class VkTarget : BrowserManager, IVkTarget
    {
        const string BROWSER_PROFILE_SERVICE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\PerformanceTasks\\VkTarget\\";

        /// <summary>
        /// Авторизация соц. сетей
        /// </summary>
        private static bool _isAuthorizationSocialNetworks;
        /// <summary>
        /// Логин для VkTarget
        /// </summary>
        private static string _loginVkTarget;
        /// <summary>
        /// Пароль для VkTarget
        /// </summary>
        private static string _passwordVkTarget;

        /// <summary>
        /// Тип соц. сети
        /// </summary>
        private string _typeSocialNetwork;
        /// <summary>
        /// Задача
        /// </summary>
        private string _task;
        /// <summary>
        /// Url-страницы,
        /// </summary>
        private string _urlByTask;
        /// <summary>
        /// Количество действий
        /// </summary>
        private int _countAction;

        private IVkManager _vkManager;
        private IYouTubeManager _ytManager;
        private IClassmatesManager _classmatesManager;
        private IYandexZenManager _yandexZenManager;
        private ITumblr _tumblrManager;
        private IReddit _redditManager;
        private IQuora _quoraManager;
        private ISoundCloud _soundCloudManager;
        private IVimeoManager _vimeoManager;

        protected void Init()
        {
            Initialization(BROWSER_PROFILE_SERVICE);
            SetContextForManagers();

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
            _ytManager = new YouTubeManager();
            _classmatesManager = new ClassmatesManager();
            _yandexZenManager = new YandexZenManager();
            _tumblrManager = new TumblrManager();
            _redditManager = new RedditManager();
            _quoraManager = new QuoraManager();
            _soundCloudManager = new SoundCloudManager();
            _vimeoManager = new VimeoManager();

            var driver = GetDriver();
            _vkManager.SetContextBrowserManager(driver);
            _ytManager.SetContextBrowserManager(driver);
            _classmatesManager.SetContextBrowserManager(driver);
            _yandexZenManager.SetContextBrowserManager(driver);
            _tumblrManager.SetContextBrowserManager(driver);
            _redditManager.SetContextBrowserManager(driver);
            _quoraManager.SetContextBrowserManager(driver);
            _soundCloudManager.SetContextBrowserManager(driver);
            _vimeoManager.SetContextBrowserManager(driver);
        }

        /// <summary>
        /// Авторизация в соц сетях
        /// </summary>
        protected void AuthorizationSocialNetworks()
        {
            var accounts = AccountService.GetAccount(TypeService.VkTarget);

            var accountMain = accounts.Where(w => w.AccountType == AccountType.Main).First();
            _loginVkTarget = accountMain.Login;
            _passwordVkTarget = accountMain.Password;

            var accountVK = accounts.Where(w => w.AccountType == AccountType.Vk).FirstOrDefault();
            if (accountVK != null)
            {
                _vkManager.Authorization(accountVK.Login, accountVK.Password);
            }

            var accountReddit = accounts.Where(w => w.AccountType == AccountType.Reddit).FirstOrDefault();
            if (accountReddit != null)
            {
                _redditManager.Authorization(accountReddit.Login, accountReddit.Password);
            }

            var accountYouTube = accounts.Where(w => w.AccountType == AccountType.YouTube).FirstOrDefault();
            if (accountYouTube != null)
            {
                _ytManager.Authorization(accountYouTube.Login, accountYouTube.Password);
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

            var accountSoundCloud = accounts.Where(w => w.AccountType == AccountType.SoundCloud).FirstOrDefault();
            if (accountSoundCloud != null)
            {
                _soundCloudManager.Authorization(accountSoundCloud.Login, accountSoundCloud.Password);
            }

            _isAuthorizationSocialNetworks = true;
        }

        public void GoTo(string url)
        {
            Init();
            GoToUrl(url);
            AuthorizationOnService();
            BeginCollecting(url);
        }

        public void Quit()
        {
            UpdateModel(GetUrlPage());
            QuitBrowser();
        }

        /// <summary>
        /// Начать сбор
        /// </summary>
        protected void BeginCollecting(string url)
        {
            while (true)
            {
                GetTask();
                switch (_typeSocialNetwork)
                {
                    case "vk":
                        CarryOutTaskInVk(_task);
                        break;
                    case "youtube":
                        CarryOutTaskInYouTube(_task);
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
                    case "soundcloud":
                        CarryOutTaskInSoundCloud(_task);
                        break;
                    case "quora":
                        CarryOutTaskInQuora(_task);
                        break;
                    case "NoTasks":
                        ShowActivity();
                        break;
                    default:
                        break;
                }

                _typeSocialNetwork = string.Empty;
                _task = string.Empty;
                _urlByTask = string.Empty;

                UpdateModel(url);
            }
        }

        /// <summary>
        /// Выполнить задачу в VK
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInVk(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            bool isError = false;
            switch (taskText)
            {
                case "Вступите в сообщество":
                    if (_vkManager.IsBlockedCommunity())
                    {
                        isError = true;
                        break;
                    }
                    _vkManager.JoinToComunity();
                    break;
                case "Поставьте лайк на странице":
                    if (!_vkManager.IsPostFound())
                    {
                        isError = true;
                        break;
                    }
                    _vkManager.PutLike();
                    break;
                case "Посмотреть пост":
                    Thread.Sleep(500);
                    break;
                case "Нажмите поделиться записью":
                    if (!_vkManager.IsPostFound())
                    {
                        isError = true;
                        break;
                    }
                    _vkManager.MakeRepost();
                    break;
                case "Добавить в друзья":
                    if (_vkManager.IsBlockedAccount())
                    {
                        isError = true;
                        break;
                    }
                    _vkManager.AddToFriends();
                    break;
                case "Расскажите о группе":
                    if (_vkManager.IsBlockedCommunity())
                    {
                        isError = true;
                        break;
                    }
                    _vkManager.ToTellAboutGroup();
                    break;
                default:
                    break;
            }

            CloseTab();
            SwitchToTab();

            if (isError)
            {
                SkipTask();
                return;
            }

            CheckTask();
        }
        /// <summary>
        /// Выполнить задачу в YouTube
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInYouTube(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            bool isError = false;
            switch (taskText)
            {
                case "Подпишитесь на канал":
                    _ytManager.SubscribeToChannel();
                    break;
                case "Поставьте 'Лайк' под видео":
                    if (!_ytManager.IsVideoAvailable())
                    {
                        isError = true;
                        break;
                    }
                    _ytManager.LikeUnderVideo();
                    break;
                case "Поставьте 'Не нравится' под видео":
                    if (!_ytManager.IsVideoAvailable())
                    {
                        isError = true;
                        break;
                    }
                    _ytManager.DislikeUnderVideo();
                    break;
                default:
                    break;
            }

            CloseTab();
            SwitchToTab();

            if (isError)
            {
                SkipTask();
                return;
            }

            CheckTask();
        }
        /// <summary>
        /// Выполнить задачу в Сlassmates
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInСlassmates(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            switch (taskText)
            {
                case "Вступить в группу":
                    _classmatesManager.JoinGroup();
                    break;
                case "Поставьте класс под записью":
                    //TODO: Объект может быть блокирован https://ok.ru/group/49779642269757/topic/152379496667944
                    _classmatesManager.PutClass();
                    break;
                case "Поставить 'Класс' на публикации":
                    _classmatesManager.PutClass();
                    break;
                case "Поделиться записью":
                    _classmatesManager.MakeRepost();
                    break;
                default:
                    break;
            }

            CloseTab();
            SwitchToTab();
            CheckTask();
        }
        /// <summary>
        /// Выполнить задачу в Yandex.Zen
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInZen(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            switch (taskText)
            {
                case "Поставьте лайк на пост":
                    _yandexZenManager.PutLike();
                    break;
                case "Подпишитесь на пользователя":
                    _yandexZenManager.Subscribe();
                    break;
                default:
                    break;
            }

            CloseTab();
            SwitchToTab();
            CheckTask();
        }
        /// <summary>
        /// Выполнить задачу в Reddit
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInReddit(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            switch (taskText)
            {
                case "Нажать стрелку вверх на Запись":
                    _redditManager.UpArrowForPost();
                    break;
                case "Подпишитесь на пользователя":
                    var panel = GetElementByClassName("bDDEX4BSkswHAG_45VkFB");
                    if (panel != null)
                    {
                        panel.FindElement(SearchMethod.Tag, "button").Click();
                        Thread.Sleep(2500);
                    }
                    _redditManager.Subscribe();
                    break;
                default:
                    break;
            }

            CloseTab();
            SwitchToTab();
            CheckTask();
        }
        /// <summary>
        /// Выполнить задачу в Tumblr
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInTumblr(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            switch (taskText)
            {
                case "Реблогните блог":
                    _tumblrManager.Reblog();
                    break;
                case "Поставить лайк на пост":
                    _tumblrManager.LikePost();
                    break;
                default:
                    break;
            }

            CloseTab();
            SwitchToTab();
            CheckTask();
        }
        /// <summary>
        /// Выполнить задачу в SoundCloud
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInSoundCloud(string taskText)
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            switch (taskText)
            {
                case "подпишитесь на аккаунт":
                    _soundCloudManager.Subscribe();
                    break;
                case "Поставить лайк на трэк":
                    _soundCloudManager.LikeTrack();
                    break;
                case "Поделиться трэком":
                    _soundCloudManager.RepostTrack();
                    break;
                case "Воспроизвести трэк":
                    Thread.Sleep(500);
                    break;
                default:
                    break;
            }

            CloseTab();
            SwitchToTab();
            CheckTask();
        }
        /// <summary>
        /// Выполнить задачу в Quora
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInQuora(string taskText) //TODO
        {
            SwitchToLastTab();
            _urlByTask = GetUrlPage();

            switch (taskText)
            {
                case "Подписаться на пользователя":
                    _quoraManager.Subscribe(); //https://www.quora.com/profile/Brajesh-Kumar-Singh-4
                    break;
                case "Поставьте лайк на ответ":
                    _quoraManager.LikeAnswer();
                    break;
                default:
                    break;
            }

            CloseTab();
            SwitchToTab();
            CheckTask();
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
                case "youtube":
                    UndoTaskInYouTube();
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
                case "soundcloud":
                    UndoTaskInSoundCloud();
                    break;
                case "tumblr":
                    UndoTaskInTumblr();
                    break;
            }
        }
        /// <summary>
        /// Отменить задачу в Vk
        /// </summary>
        protected void UndoTaskInVk()
        {
            OpenPageInNewTab(_urlByTask);

            switch (_task)
            {
                case "Вступите в сообщество":
                    _vkManager.UnsubscribeToComunity();
                    break;
                case "Поставьте лайк на странице":
                    _vkManager.RemoveLike();
                    break;
                case "Добавить в друзья":
                    _vkManager.RemoveFromFriends();
                    break;
            }

            CloseTab();
            SwitchToTab();
        }
        /// <summary>
        /// Отменить задачу в YouTube
        /// </summary>
        protected void UndoTaskInYouTube()
        {
            OpenPageInNewTab(_urlByTask);

            switch (_task)
            {
                case "Подпишитесь на канал":
                    _ytManager.UnsubscribeFromChannel();
                    break;
                case "Поставьте 'Лайк' под видео":
                    _ytManager.RemoveLike();
                    break;
                case "Поставьте 'Не нравится' под видео":
                    _ytManager.RemoveDislike();
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
            OpenPageInNewTab(_urlByTask);

            switch (_task)
            {
                case "Вступить в группу":
                    _classmatesManager.LeaveGroup();
                    break;
                case "Поставьте класс под записью":
                case "Поставить 'Класс' на публикации":
                    _classmatesManager.RemoveClass();
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
            OpenPageInNewTab(_urlByTask);

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
            OpenPageInNewTab(_urlByTask);

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
            OpenPageInNewTab(_urlByTask);

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
        /// Отменить задачу в SoundCloud
        /// </summary>
        protected void UndoTaskInSoundCloud()
        {
            OpenPageInNewTab(_urlByTask);

            switch (_task)
            {
                case "подпишитесь на аккаунт":
                    _soundCloudManager.Unsubscribe();
                    break;
                case "Поставить лайк на трэк":
                    _soundCloudManager.RemoveLike();
                    break;
                case "Поделиться трэком":
                    _soundCloudManager.RemoveRepost();
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
            OpenPageInNewTab(_urlByTask);

            switch (_task)
            {
                case "Поставить лайк на пост":
                    _tumblrManager.RemoveLike();
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
                case ActionToBrowser.RefreshPage:
                    if (!isRefresh)
                    {
                        break;
                    }
                    else if (randomNumber == 2)
                    {
                        Refresh();
                        break;
                    }
                    FocusOnFirstElementMenu();
                    break;
                case ActionToBrowser.ClickOnElement:
                    if (randomNumber == 2)
                    {
                        ClickOnElementMenu();
                        break;
                    }
                    FocusOnFirstElementMenu();
                    break;
                case ActionToBrowser.ClickOnElements:
                    if (randomNumber == 2)
                    {
                        ClickOnElementsMenu();
                        break;
                    }
                    FocusOnAllElementsMenu();
                    break;
                case ActionToBrowser.Inaction:
                    if (_countAction >= 40)
                    {
                        Inaction();
                        _countAction = 0;
                        return;
                    }
                    break;
            }

            int randomMilliseconds = GetRandomNumber(1000, 5000);
            int randomSleep = GetRandomNumber(2, 4) * randomMilliseconds;
            Thread.Sleep(randomSleep);
        }
        /// <summary>
        /// Пропуск задачу
        /// </summary>
        protected void SkipTask()
        {
            ExecuteScript("var task = document.querySelector('#list>main>section:nth-child(3)>div>div>div>div:nth-child(1)>" +
                "div.container-fluid.available__table').getElementsByClassName('row tb__row');" +
                "task[0].children[5].getElementsByClassName('control__item close')[0].click();");
        }
        /// <summary>
        /// Проверить задачу
        /// </summary>
        protected void CheckTask() //TODO: Переработать метод. Разбить на несколько
        {
            string getTaskScript = "var task = document.querySelector('#list>main>section:nth-child(3)>div>div>div>div:nth-child(1)>div" +
                ".container-fluid.available__table').getElementsByClassName('row tb__row')[0];";
            string clickButtonScript = "task.children[3].getElementsByClassName('default__small__btn check__btn')[0].click();";
            string getAttribute = "return task.getAttribute('data-task-item');";

            var taksId = ExecuteScript(getTaskScript + clickButtonScript + getAttribute);
            Thread.Sleep(1500);

            int waitingСounter = 0;

            bool isCheked = true;
            while (isCheked)
            {
                var errorPanel = GetElementByClassName("is_error");
                if (errorPanel != null)
                {
                    var dataId = errorPanel.GetDataId();
                    if (dataId != taksId)
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
                            waitingСounter++;
                            if (waitingСounter < 3)
                            {
                                Thread.Sleep(15000);
                                ExecuteScript(getTaskScript + clickButtonScript);
                                continue;
                            }

                            SkipTask();
                            UndoTask();
                            return;
                        case "Увы, но список заданий устарел! Скоро вы получите новые задания.":
                            SkipTask();
                            UndoTask();
                            return;
                    }
                }
                else if (taksId != ExecuteScript(getTaskScript + getAttribute))
                {
                    isCheked = false;
                }
                else
                {
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
                "var taskEmpty = document.querySelector('#list>main>section:nth-child(2)>div>div>div>div:nth-child(1)>div.empty');" +
                "if (taskEmpty.classList.length == 1)" +
                "{" +
                "return 'NoTasks'" +
                "}" +
                "var tasks = document.querySelector('#list>main>section:nth-child(2)>div>div>div>div:nth-child(1)>div.container-fluid" +
                ".available__table').getElementsByClassName('row tb__row');" +
                "var systemType = tasks[0].getElementsByClassName('social__img')[0].class.toString().replace('social__img ', '');" +
                "var button = tasks[0].children[2].getElementsByTagName('a')[0];" +
                "var task = document.getElementsByClassName('wrap')[1].innerText;" +
                "button.click();" +
                "return systemType + '|' + task;").Split("|");

            _typeSocialNetwork = taskDetails[0];

            if (taskDetails.Length == 2)
            {
                _task = taskDetails[1];
            }
        }
        /// <summary>
        /// Бездействие
        /// </summary>
        protected void Inaction()
        {
            string getTaskScript = "var taskEmpty = document.querySelector('#list>main>section:nth-child(3)>div>div>div>div:nth-child" +
                "(1)>div.empty');if (taskEmpty.classList.length == 1){return 'false'}else{return 'true'}";
            int randomTimerMinuts = GetRandomNumber(10, 61);

            while (true)
            {
                bool taskExists = Convert.ToBoolean(ExecuteScript(getTaskScript));
                if (taskExists)
                {
                    return;
                }
                else if (randomTimerMinuts == 0)
                {
                    return;
                }
                else
                {
                    Thread.Sleep(60000);
                    randomTimerMinuts--;
                }
            }
        }


        /// <summary>
        /// Авторизация на сервисе
        /// </summary>
        /// <param name="url">Url-адрес сервиса</param>
        protected void AuthorizationOnService()
        {
            if (GetUrlPage() == "https://users.vktarget.ru/list/")
            {
                return;
            }

            GetElementsByClassName("mdl-js-button").Last().Click();
            Thread.Sleep(1500);

            var login = GetElementByXPath("//*[@id='login_form']/div[1]/div/div[2]/div[2]/input");
            if (string.IsNullOrWhiteSpace(login.Text))
            {
                login.SendKeys(_loginVkTarget);
            }

            var password = GetElementByXPath("//*[@id='login_form']/div[1]/div/div[3]/div[2]/input");
            if (string.IsNullOrWhiteSpace(password.Text))
            {
                password.SendKeys(_passwordVkTarget);
            }

            ExecuteScript("document.getElementById('login_form').getElementsByTagName('button')[0].click()");
            Thread.Sleep(2500);
        }
        /// <summary>
        /// Получить баланс
        /// </summary>
        protected string GetBalance()
        {
            return GetElementByXPath("//*[@id='header']/div/div/div[3]/div/div/div[2]/span[2]").GetInnerText() + "руб";
        }
        /// <summary>
        /// Обновить модель
        /// </summary>
        /// <param name="url">Url-адрес</param>
        protected void UpdateModel(string url)
        {
            var internetService = WebService.GetInternetServices().Where(w => w.URL == url).FirstOrDefault();
            internetService.BalanceOnService = GetBalance();

            WebService.UpdateInternetService(internetService);
        }


        /// <summary>
        /// Получить случайное число
        /// </summary>
        /// <param name="min">Минимальное значение</param>
        /// <param name="max">Максимальное значение</param>
        /// <returns>Случайное числовое значение</returns>
        protected int GetRandomNumber(int min, int max)
        {
            return new Random().Next(min, max);
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
                item.Click();
                Thread.Sleep(1500);
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

            Thread.Sleep(1000);
            logotype.Click();
        }
        /// <summary>
        /// Получить действие
        /// </summary>
        /// <returns>Действие в браузере</returns>
        protected ActionToBrowser GetAction()
        {
            int randomAction = GetRandomNumber(0, 7);
            switch (randomAction)
            {
                case 0:
                    return ActionToBrowser.FocusOnElement;
                case 1:
                    return ActionToBrowser.FocusOnElements;
                case 2:
                    return ActionToBrowser.FocusOnTab;
                case 3:
                    return ActionToBrowser.RefreshPage;
                case 4:
                    return ActionToBrowser.Inaction;
                case 5:
                    return ActionToBrowser.ClickOnElement;
                case 6:
                    return ActionToBrowser.ClickOnElements;
                default:
                    return ActionToBrowser.FocusOnElement;
            }
        }
    }
}
