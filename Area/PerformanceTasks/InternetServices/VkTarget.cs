using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.Managers.Interface;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Models;
using OpenQA.Selenium;
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
            
            var driver = GetDriver();
            _vkManager.SetContextBrowserManager(driver);
            _ytManager.SetContextBrowserManager(driver);
        }

        protected void AuthorizationSocialNetworks()
        {
            var accounts = AccountService.GetAccount(TypeService.VkTarget);
            var accountMain = accounts.Where(w => w.AccountType == AccountType.Main).First();
            _login = accountMain.Login;
            _password = accountMain.Password;

            //_loginClassmates = _login;
            //_passwordClassmates = _password;

            var accountVK = accounts.Where(w => w.AccountType == AccountType.Vk).First();
            var accountYouTube = accounts.Where(w => w.AccountType == AccountType.YouTube).First();

            _vkManager.Authorization(accountVK.Login, accountVK.Password);
            _ytManager.Authorization(accountYouTube.Login, accountYouTube.Password);
            //TODO: Авторизация одноклассники
            _isAuthorization = true;
        }

        public void GoTo(string url)
        {
            Init();
            GoToUrl(url);
            AuthorizationOnService(url);
            BeginCollecting();
        }

        public void Quit() //ЕСТЬ TODO
        {
            var service = WebService.GetInternetServices().Where(w => w.URL == GetUrlPage()).FirstOrDefault();
            //TODO:Получение баланса с сервиса
            WebService.UpdateInternetService(service);
            QuitBrowser();
        }

        /// <summary>
        /// Начать сбор
        /// </summary>
        protected void BeginCollecting()
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
                    case "NoTasks":
                        OnFocusElement();
                        Thread.Sleep(60000);
                        break;
                }
            }
        }

        protected void OnFocusElement()
        {
            var button = GetElementByXPath("//*[@id='list']/main/section[1]/div[2]/div/div/div/ul/li[1]");
            FocusOnElement(button);
        }

        /// <summary>
        /// Выполнить задачу в вк
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInVk(string taskText)
        {
            SwitchToLastTab();

            switch (taskText)
            {
                case "Вступите в сообщество":
                    if (_vkManager.IsBlockedCommunity())
                    {
                        CloseTab();
                        SwitchToTab();
                        SkipTask();
                        return;
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
                    _vkManager.MakeRepost();
                    break;
                case "Добавить в друзья":
                    //TODO: Реализовать добавление в други
                    break;
            }

            CloseTab();
            SwitchToTab();
            CheckTask();
        }

        /// <summary>
        /// Выполнить задачу в ютуб
        /// </summary>
        /// <param name="taskText">Текст задачи</param>
        protected void CarryOutTaskInYouTube(string taskText)
        {
            SwitchToLastTab();

            switch (taskText)
            {
                case "Подпишитесь на канал":
                    _ytManager.SubscribeToChannel();
                    break;
                case "Поставьте 'Лайк' под видео":
                    _ytManager.LikeUnderVideo();
                    break;
                case "Поставьте 'Не нравится' под видео":
                    _ytManager.DislikeUnderVideo();
                    break;
            }

            CloseTab();
            SwitchToTab();
            CheckTask();
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
        protected void CheckTask()
        {
            ExecuteScript("var task = document.querySelector('#list>main>section:nth-child(3)>div>div>div>div:nth-child(1)>" +
                "div.container-fluid.available__table').getElementsByClassName('row tb__row');" +
                "task[0].children[3].getElementsByClassName('default__small__btn check__btn')[0].click();");
            Thread.Sleep(4000);
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

        protected void AddToFrend(IWebElement webElement)
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
        }
    }
}
