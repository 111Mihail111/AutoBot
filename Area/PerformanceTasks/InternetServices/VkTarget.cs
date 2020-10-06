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

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class VkTarget : BrowserManager, IVkTarget
    {
        const string BROWSER_PROFILE_SERVICE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\PerformanceTasks\\VkTarget\\";

        private static bool _isAuthorization;
        private string _login;
        private string _password;

        private IVkManager _vkManager;
        private IYouTubeManager _ytManager;
        private IClassmatesManager _classmatesManager;
        private IYandexZenManager _yandexZenManager;

        protected void Init()
        {
            Initialization(BROWSER_PROFILE_SERVICE);
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
            _ytManager = new YouTubeManager();
            _classmatesManager = new ClassmatesManager();
            _yandexZenManager = new YandexZenManager();

            var driver = GetDriver();
            _vkManager.SetContextBrowserManager(driver);
            _ytManager.SetContextBrowserManager(driver);
            _classmatesManager.SetContextBrowserManager(driver);
            _yandexZenManager.SetContextBrowserManager(driver);
        }

        /// <summary>
        /// Авторизация в соц сетях
        /// </summary>
        protected void AuthorizationSocialNetworks()
        {
            var accounts = AccountService.GetAccount(TypeService.VkTarget);

            var accountMain = accounts.Where(w => w.AccountType == AccountType.Main).First();
            _login = accountMain.Login;
            _password = accountMain.Password;

            var accountVK = accounts.Where(w => w.AccountType == AccountType.Vk).FirstOrDefault();
            if (accountVK != null)
            {
                _vkManager.Authorization(accountVK.Login, accountVK.Password);
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

            _isAuthorization = true;
        }

        public void GoTo(string url)
        {
            Init();
            GoToUrl(url);
            AuthorizationOnService(url);
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
                var task = GetTask();
                switch (task[0])
                {
                    case "vk":
                        CarryOutTaskInVk(task[1]);
                        break;
                    case "youtube":
                        CarryOutTaskInYouTube(task[1]);
                        break;
                    case "odnoklassniki":
                        CarryOutTaskInСlassmates(task[1]);
                        break;
                    case "zen":
                        CarryOutTaskInZen(task[1]);
                        break;
                    case "NoTasks":
                        ShowActivity();
                        break;
                    default:
                        break;
                }

                UpdateModel(url);
            }
        }

        /// <summary>
        /// Выполнить задачу в вк
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInVk(string taskText)
        {
            SwitchToLastTab();

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
                    _vkManager.PutLike();
                    break;
                case "Посмотреть пост":
                    Thread.Sleep(1500);
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
        /// Выполнить задачу в ютуб
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInYouTube(string taskText)
        {
            SwitchToLastTab();

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
        /// Выполнить задачу в одноклассниках
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInСlassmates(string taskText) //Есть TODO
        {
            SwitchToLastTab();

            switch (taskText)
            {
                case "Вступить в группу":
                    _classmatesManager.JoinGroup();
                    break;
                case "Поставьте класс под записью":
                    _classmatesManager.PutClass();
                    break;
                case "Поставить 'Класс' на публикации":
                    _classmatesManager.PutClass(); //TODO: Протестить https://ok.ru/vismarketru/topic/152066788526855
                    break;
                default:
                    break;
            }

            CloseTab();
            SwitchToTab();
            CheckTask();
        }

        /// <summary>
        /// Выполнить задачу в Яндекс.Дзен
        /// </summary>
        /// <param name="taskText"></param>
        protected void CarryOutTaskInZen(string taskText)
        {
            SwitchToLastTab();

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
        /// Проявить активность
        /// </summary>
        protected void ShowActivity() //TODO: Довести до ума метод
        {
            int randomSleep = GetRandomNumber(1, 3) * 5000;

            int randomAction = GetRandomNumber(1, 5);
            switch (randomAction)
            {
                case 1:
                    var liCollection = GetElementByXPath("//*[@id='list']/main/section[1]/div[2]/div/div/div/ul")
                        .FindElements(SearchMethod.Tag, "li").ToList();
                    var randomIndex = GetRandomNumber(0, liCollection.Count);
                    var liElement = liCollection[randomIndex];

                    FocusOnElement(liElement);

                    if (liElement.GetInnerText().Contains("Доступные") && randomAction == 1)
                    {
                        liElement.Click();
                        Thread.Sleep(randomSleep + 5000 / randomAction);
                    }
                    break;
                case 2:
                    var logotype = GetElementByXPath("//*[@id='header']/div/div/div[1]/div/a");
                    FocusOnElement(logotype);

                    if (randomAction == 1)
                    {
                        logotype.Click();
                        Thread.Sleep(randomSleep + 5000 / randomAction);
                    }
                    break;
                case 3:
                    //OpenPageInNewTab(string.Empty);
                    Thread.Sleep(randomSleep + 5000 / randomAction);

                    //CloseTab();
                    SwitchToTab();
                    break;
                case 4:
                    Thread.Sleep(randomSleep); //TODO: Иногда мы должны засыпать минут на 5-25, 
                                               //но при этом должны асинхронно мониторить задачи и выполнять. После выполнения опять
                                               //проявлять активность и засыпать
                    break;
            }

            Thread.Sleep(randomSleep);
        }

        /// <summary>
        /// Пропуск задания
        /// </summary>
        protected void SkipTask()
        {
            ExecuteScript("var task = document.querySelector('#list>main>section:nth-child(3)>div>div>div>div:nth-child(1)>" +
                "div.container-fluid.available__table').getElementsByClassName('row tb__row')" +
                "task[0].children[5].getElementsByClassName('control__item close')[0].click();");
        }

        /// <summary>
        /// Проверить задание
        /// </summary>
        protected void CheckTask() //Есть TODO
        {
            string getTaskScript = "var task = document.querySelector('#list>main>section:nth-child(3)>div>div>div>div:nth-child(1)>div" +
                ".container-fluid.available__table').getElementsByClassName('row tb__row')[0];";
            string clickButtonScript = "task.children[3].getElementsByClassName('default__small__btn check__btn')[0].click();";
            string getAttribute = "return task.getAttribute('data-task-item');";

            var taksId = ExecuteScript(getTaskScript + clickButtonScript + getAttribute);

            bool isCheked = true;
            while (isCheked)
            {
                Thread.Sleep(1500);

                if (taksId != ExecuteScript(getTaskScript + getAttribute))
                {
                    isCheked = false;
                }
                else
                {
                    ShowActivity();
                }
            }

            //TODO:Нужно придумыть способ, как отменять проделанные действия, в случае не оплаты задания
            //document.getElementsByClassName("is_error") Если null и нет строки с taskId, значит за задание уплотили
        }

        /// <summary>
        /// Получить задачу
        /// </summary>
        /// <returns></returns>
        protected string[] GetTask()
        {
            return ExecuteScript(
                "var taskEmpty = document.querySelector('#list>main>section:nth-child(3)>div>div>div>div:nth-child(1)>div.empty');" +
                "if (taskEmpty.classList.length == 1)" +
                "{" +
                "return 'NoTasks'" +
                "}" +
                "var tasks = document.querySelector('#list>main>section:nth-child(3)>div>div>div>div:nth-child(1)>div.container-fluid" +
                ".available__table').getElementsByClassName('row tb__row');" +
                "var systemType = tasks[0].getElementsByClassName('social__img')[0].class.toString().replace('social__img ', '');" +
                "var button = tasks[0].children[2].getElementsByTagName('a')[0];" +
                "var task = document.getElementsByClassName('wrap')[1].innerText;" +
                "button.click();" +
                "return systemType + '|' + task;").Split("|");
        }

        protected void AddToFrend(IWebElement webElement) //Есть TODO
        {
            webElement.Click();

            SwitchToLastTab();
            GetElementByClassName("flat_button button_wide").Click(); //TODO:Вызов менеджера и добавление в друзья
        }

        /// <summary>
        /// Авторизация на сервисе
        /// </summary>
        /// <param name="url">Url-адрес сервиса</param>
        protected void AuthorizationOnService(string url)
        {
            if (GetUrlPage() == url)
            {
                return;
            }

            var login = GetElementByXPath("//*[@id='login_form']/div[1]/div/div[2]/div[2]/input");
            if (string.IsNullOrWhiteSpace(login.Text))
            {
                login.SendKeys(_login);
            }

            var password = GetElementByXPath("//*[@id='login_form']/div[1]/div/div[3]/div[2]/input");
            if (string.IsNullOrWhiteSpace(password.Text))
            {
                password.SendKeys(_password);
            }

            GetElementByXPath("//*[@id='login_form']/div[2]/div/div/div[1]/button").Click();
            Thread.Sleep(2000);
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

        protected int GetRandomNumber(int min, int max)
        {
            return new Random().Next(min, max);
        }

    }
}
