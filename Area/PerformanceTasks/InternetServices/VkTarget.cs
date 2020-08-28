using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Extentions;
using AutoBot.Models;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class VkTarget : BrowserManager, IVkTarget
    {                                          
        const string BROWSER_PROFILE_SERVICE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\PerformanceTasks\\VkTarget\\";
        private string _login;
        private string _password;
        private string _loginVK;
        private string _passwordVK;
        private string _loginClassmates;
        private string _passwordClassmates;
        private string _loginYouTube;
        private string _passwordYouTube;

        protected void Init()
        {
            var accounts = AccountService.GetAccount(TypeService.VkTarget);

            var accountMain = accounts.Where(w => w.AccountType == AccountType.Main).First();
            _login = accountMain.Login;
            _password = accountMain.Password;

            _loginClassmates = _login;
            _passwordClassmates = _password;

            var accountVK = accounts.Where(w => w.AccountType == AccountType.Vk).First();
            _loginVK = accountVK.Login;
            _passwordVK = accountVK.Password;

            var accountYouTube = accounts.Where(w => w.AccountType == AccountType.YouTube).First();
            _loginYouTube = accountYouTube.Login;
            _passwordYouTube = accountYouTube.Password;

            Initialization(BROWSER_PROFILE_SERVICE);
        }

        public void GoTo(string url)
        {
            Init();
            GoToUrl(url);
            AuthorizationOnService(url);
            BeginCollecting();
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
                        Thread.Sleep(60000);
                        break;
                }
            }
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
                    JoinToComunity();
                    break;
                case "Поставьте лайк на странице":
                    break;
            }

            CloseTab();
            SwitchToTab();
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
                    break;
                case "Поставьте 'Лайк' под видео":
                    break;
                case "Поставьте 'Не нравится' под видео":
                    break;
            }

            CloseTab();
            SwitchToTab();
        }

        /// <summary>
        /// Получить задачу
        /// </summary>
        /// <returns></returns>
        protected string[] GetTask()
        {
            return ExecuteScript(
                "var tasks = document.getElementsByClassName('row tb__row last');" +
                "if (tasks.length === 0)" +
                "{" +
                "return 'NoTasks'" +
                "}" +
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
            AuthorizationVK();
            GetElementByClassName("flat_button button_wide").Click(); //TODO:Вызов менеджера и добавление в друзья
        }

        protected void JoinToComunity()
        {
            AuthorizationVK();

            var joinButton = GetElementById("join_button"); //TODO:Заменить код ниже на вызов из менеджера
            if (joinButton == null)
            {
                GetElementById("public_subscribe").Click();
                Thread.Sleep(1500);
                return;
            }
            joinButton.Click();
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Авторизация
        /// </summary>
        protected void AuthorizationVK()
        {
            if (GetElementById("quick_login") == null)
            {
                return;
            }

            //TODO:Код ниже заменится на вызов менеджера
            GetElementById("quick_email").SendKeys(_loginVK);
            GetElementById("quick_pass").SendKeys(_passwordVK);
            GetElementById("quick_login_button").Click();
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
            if (login.Text != string.Empty)
            {
                login.SendKeys(_login);
            }

            var password = GetElementByXPath("//*[@id='login_form']/div[1]/div/div[3]/div[2]/input");
            if (password.Text != string.Empty)
            {
                password.SendKeys(_password);
            }

            GetElementByXPath("//*[@id='login_form']/div[2]/div/div/div[1]/button").Click();
        }
    }
}
