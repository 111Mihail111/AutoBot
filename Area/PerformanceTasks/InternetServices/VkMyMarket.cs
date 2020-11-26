using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.Managers.Interface;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Extentions;
using AutoBot.Models;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class VkMyMarket : BrowserManager, IVkMyMarket
    {
        const string BROWSER_PROFILE_SERVICE = "C:\\_AutoBot\\Profiles\\PerformanceTasks\\VkMyMarket\\";

        /// <summary>
        /// Авторизация соц. сетей
        /// </summary>
        private static bool _isAuthorizationSocialNetworks;
        /// <summary>
        /// Задание
        /// </summary>
        protected IWebElement Task;

        private IVkManager _vkManager;
        private IYouTubeManager _ytManager;
        private IClassmatesManager _classmatesManager;

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
            

            var driver = GetDriver();
            _vkManager.SetContextBrowserManager(driver);
            _ytManager.SetContextBrowserManager(driver);
            _classmatesManager.SetContextBrowserManager(driver);           
        }

        public void AuthorizationSocialNetworks()
        {
            var accounts = AccountService.GetAccount(TypeService.VkMyMarket);

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

            _isAuthorizationSocialNetworks = true;
        }

        public void GoTo(string url)
        {
            Init();
            GoToUrl(url);
            Authorization();
            StartJob();
        }

        protected void Authorization()
        {
            var button = GetElementByClassName("blue");
            if (!button.Displayed)
            {
                return;
            }

            button.Click();
            Thread.Sleep(1500);

            GetElementByClassName("ulogin-button-vkontakte").Click();
            Thread.Sleep(1500);
        }

        protected void StartJob()
        {
            string url = GetUrlPage();
            GetElementsByTagName("button").Where(w => w.GetTitle() == "Заработать").First().Click();

            while (true)
            {
                var task = GetTaskDetails();
                switch (task.Key)
                {
                    case "Вконтакте":
                        CarryOutTaskInVk(task.Value);
                        break;
                    case "Нет заданий":
                        //Проявление активности
                        break;
                }

                Task = null;
                UpdateModel(url);
            }            
        }

        protected KeyValuePair<string, string> GetTaskDetails()
        {
            string xPath = "/html/body/table/tbody/tr[2]/td/table/tbody/tr/td[3]/table/tbody/tr[2]/td/div[3]/table/tbody/tr/td[2]/table[1]";
            var task = GetElementByXPath(xPath);

            if (task == null)
            {
                return new KeyValuePair<string, string>(key: "Нет заданий", value: string.Empty);
            }

            GetElementById("order_action").Click();
            Task = task;

            var socialNetworkType = task.FindElement(SearchMethod.Tag, "img").GetTitle();
            var taskDescription = task.FindElement(SearchMethod.ClassName, "text_black").GetInnerText();           

            return new KeyValuePair<string, string>(key: socialNetworkType, value: taskDescription);
        }

        protected void CarryOutTaskInVk(string task)
        {
            Task.FindElement(SearchMethod.Tag, "a").Click();
            SwitchToLastTab();

            switch (task)
            {
                case "Задание: Расскажите друзьям":
                    _vkManager.MakeRepost();
                    break;
            }

            CloseTab();
            SwitchToTab();
            CheckTask();
        }

        protected void CheckTask()
        {
            Task.FindElement(SearchMethod.Id, "order_check").Click();
            Thread.Sleep(2000);

            var result = GetElementByClassName("text_lit_green").GetInnerText();
            if (result != "Задание выполнено.")
            {
                //Какое-то действие
            }

            RefreshPage();
        }

        protected void UpdateModel(string url)
        {
            var internetService = WebService.GetInternetServices().Where(w => w.URL == url).FirstOrDefault();
            internetService.BalanceOnService = GetElementByClassName("rur").GetInnerText().Split("Р").First() + "уб";

            WebService.UpdateInternetService(internetService);
        }
    }
}
