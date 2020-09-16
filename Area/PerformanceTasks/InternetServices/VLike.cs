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
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\PerformanceTasks\\V_Like\\";
        private IVkManager _vkManager;

        private string _loginVK;
        private string _passwordVK;
        private string _loginInstagram;
        private string _passwordInstagram;

        protected void Init()
        {
            var accounts = AccountService.GetAccount(TypeService.VLike);

            var accountVK = accounts.First();
            _loginVK = accountVK.Login;
            _passwordVK = accountVK.Password;

            var accountInstagram = accounts.Where(w => w.AccountType == AccountType.Instagram).First();
            _loginInstagram = accountInstagram.Login;
            _passwordInstagram = accountInstagram.Password;

            Initialization(BROWSER_PROFILE_CRANE);

            _vkManager = new VkManager();
            _vkManager.SetContextBrowserManager(GetDriver());
        }

        public InternetService GoTo(InternetService service)
        {
            Init();
            GoToUrl(service.URL);
            AuthorizationOnService();
            BeginCollecting();

            return GetDetailsWithService(service);
        }

        /// <summary>
        /// Начать сбор
        /// </summary>
        private void BeginCollecting()
        {
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
        protected void JoinInCommunityVK()
        {
            GetElementByXPath("//*[@id='vk1']/a").Click();

            var message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            while (message == null)
            {
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                SwitchToLastTab();
                RemoveWindowMessage();
                string url = GetUrlPage();

                if (_vkManager.IsPrivateGroup() || _vkManager.IsBlockedCommunity())
                {
                    SkipTask();
                    message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
                    continue;
                }

                _vkManager.JoinToComunity();

                CloseTab();
                SwitchToTab();
                GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                
                if (DelayPayments())
                {
                    OpenPageInNewTab(url);
                    _vkManager.UnsubscribeToComunity();
                    SkipTask();
                }

                message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
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
                    AlertAccept();
                    Thread.Sleep(5000);

                    counter++;
                    if (counter == 20)
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
        public void SkipTask()
        {
            CloseTab();
            SwitchToTab();
            GetElementByXPath("//*[@id='buttons']/a[1]").Click();
            Thread.Sleep(1000);
            GetElementByXPath("//*[@id='content']/div[3]/div[1]/a[1]").Click(); //У лайков есть еще такой путь //*[@id="like457756"]/div[3]/a или вот //*[@id="like457856"]/div[3]/span
        }
        
        /// <summary>
        /// Работа с лайками
        /// </summary>
        protected void WorkWithLikesVK()
        {
            GetElementByXPath("//*[@id='vk2']/a").Click();

            var perfomanse = GetElementByClassName("groups").GetInnerText();
            while (perfomanse != string.Empty && perfomanse != "Нет доступных заданий. Заходите позже!")
            {
                ButtonsVisible();
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                var titleTask = GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[2]/p").GetInnerText();
                
                SwitchToLastTab();
                RemoveWindowMessage();
                RefreshPage();
                
                if (titleTask == "Поставить Лайк + Рассказать друзьям")
                {
                    _vkManager.PutLikeAndRepost(true);
                }

                _vkManager.PutLikeAndRepost();
                
                string url = GetUrlPage();

                CloseTab();
                SwitchToTab();
                GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                Thread.Sleep(1000);

                if (IsMaxLikes())
                {
                    _vkManager.RemoveLike(url);
                    SkipTask();
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
        /// Удаление модальных окон vk (всякие тупые предложения)
        /// </summary>
        protected void RemoveWindowMessage()
        {
            ExecuteScript("document.querySelector('#box_layer_bg').remove();");
            ExecuteScript("document.querySelector('#stl_left').remove();");
            ExecuteScript("document.querySelector('#box_layer_wrap').remove()");
        }
        
        protected void WorkWithYouTube()
        {
            GetElementByXPath("//*[@id='yt0']/a").Click();

            var groups = GetElementByClassName("groups");
            while (true) //TODO:Проверка на отсутствие заданий
            {
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                SwitchToLastTab();

                if (IsVideoAvailable())
                {
                    SkipTask();
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

            var message = GetElementByXPath("//*[@id='content']/div[2]/h3").GetInnerText();
            while (message != "Никто не ищет друзей :-(")
            {
                ButtonsVisible();
                GetElementByXPath("//*[@id='friend20321']/div[3]/a").Click();
                SwitchToLastTab();

                RemoveWindowMessage();

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
                Thread.Sleep(2500);

                if (CheckPageInstagram())
                {
                    SkipTask();
                    groups = GetElementsByClassName("groups");
                    continue;
                }

                var buttons = GetElementsByTagName("button");
                for (int i = buttons.Count() - 1; i >= 0; i--)
                {
                    var button = buttons.ElementAt(i);
                    if (button.GetInnerText() == "Войти")
                    {
                        button.Click();
                        AuthorizationInInstagram();
                        buttons = GetElementsByTagName("button");
                    }
                    else if (button.GetInnerText() == "Подписаться")
                    {
                        button.Click();
                        Thread.Sleep(2000);
                        break;
                    }
                }

                CloseTab();
                SwitchToTab();
                DelayPayments(); //TODO: если true тогда отписка и пропуск задания

                Thread.Sleep(1000);
                groups = GetElementsByClassName("groups");
            }
        }

        /// <summary>
        /// Проверить страницу инстаграма
        /// </summary>
        /// <returns>True - не найдена, иначе false</returns>
        protected bool CheckPageInstagram()
        {
            return GetTitlePage().Contains("Страница не найдена") == true;
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

                if (CheckPageInstagram())
                {
                    SkipTask();
                    groups = GetElementsByClassName("groups");
                    continue;
                }

                var buttons = GetElementsByTagName("button");
                for (int i = buttons.Count() - 1; i >= 0; i--)
                {
                    var button = buttons.ElementAt(i);
                    if (button.GetInnerText() == "Войти")
                    {
                        button.Click();
                        AuthorizationInInstagram();
                        buttons = GetElementsByTagName("button");
                    }
                    else if (button.GetInnerText() == "Нравится")
                    {
                        button.Click();
                        Thread.Sleep(2000);
                        break;
                    }
                }

                CloseTab();
                SwitchToTab();
                DelayPayments(); //TODO: если true тогда отмена лайка и пропуск задания

                Thread.Sleep(2000);
                groups = GetElementsByClassName("groups");
            }
        }

        protected void AuthorizationOnService()
        {
            var button = GetElementByXPath("//*[@id='uLogin']/div");
            if (button == null)
            {
                return;
            }

            button.Click();
            Thread.Sleep(2000);

            if (GetTabsCount() > 1)
            {
                SwitchToLastTab();
                InsertLoginAndPassword();
                SwitchToTab();
            }
        }
        protected void AuthorizationInInstagram()
        {


            Thread.Sleep(1000);
            GetElementByXPath("//*[@id='loginForm']/div[1]/div[1]/div/label/input").SendKeys(_loginInstagram);
            GetElementByXPath("//*[@id='loginForm']/div[1]/div[2]/div/label/input").SendKeys(_passwordInstagram);
            GetElementByXPath("//*[@id='loginForm']/div[1]/div[3]/button/div").Click();
            GetElementByXPath("//*[@id='react-root']/section/main/div/div/div/section/div/button")?.Click();
            Thread.Sleep(1000);
        }

        protected void InsertLoginAndPassword() //TODO:Метод будет асинхронным
        {
            GetElementByXPath("//*[@id='login_submit']/div/div/input[6]").SendKeys(_loginVK); //TODO:Создать 2 новых метода. Второй асинхронный
            GetElementByXPath("//*[@id='login_submit']/div/div/input[7]").SendKeys(_passwordVK); //TODO:Создать 2 новых метода. Второй асинхронный
            GetElementById("install_allow").Click();
        }

        protected bool IsError()
        {
            var errorMessage = GetElementByXPath("//*[@id='content']/div/div[1]").GetInnerText();
            if (errorMessage == "Ошибка")
            {
                return true;
            }

            return false;
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
