using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
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
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\PerformanceTasks\\V_Like\\";
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

                if (IsPrivateGroup())
                {
                    SkipTask();
                    message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
                    continue;
                }

                JoinGroup();
                CloseTab();
                SwitchToTab();
                GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                DelayPayments();

                message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            }
        }
        /// <summary>
        /// Вступить в группу
        /// </summary>
        public void JoinGroup()
        {
            var joinButton = GetElementById("join_button");
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
        /// Задержка платежа
        /// </summary>
        public void DelayPayments()
        {
            try
            {
                var modal = GetElementById("modal");
                while (modal.Displayed)
                {
                    GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                    AlertAccept();
                    Thread.Sleep(4000);
                }
            }
            catch
            {
                //Alert'a нет, значит модал. окно закрылось
            }
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
            GetElementByXPath("//*[@id='content']/div[3]/div[1]/a[1]").Click();
        }
        /// <summary>
        /// Частная группа
        /// </summary>
        public bool IsPrivateGroup()
        {
            if (GetTitlePage() == "Частная группа")
            {
                return true;
            }

            return false;
        }



        /// <summary>
        /// Работа с лайками
        /// </summary>
        protected void WorkWithLikesVK()
        {
            GetElementByXPath("//*[@id='vk2']/a").Click();

            var perfomanse = GetElementByClassName("groups").GetInnerText();
            if (perfomanse != string.Empty && perfomanse != "Нет доступных заданий. Заходите позже!")
            {
                ButtonsVisible();
            }

            while (perfomanse != string.Empty && perfomanse != "Нет доступных заданий. Заходите позже!")
            {
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();
                var titleTask = GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[2]/p").GetInnerText();
                SwitchToLastTab();
                Thread.Sleep(500);
                RemoveWindowMessage();
                LikeItAndMakeRepost(titleTask);

                string url = GetUrlPage();
                CloseTab();
                SwitchToTab();
                Thread.Sleep(5000);
                GetElementByXPath("//*[@id='buttons']/a[2]").Click();

                if (IsMaxLikes())
                {
                    TakeOffLike(url);
                    SkipTask();
                    perfomanse = GetElementByClassName("groups").GetInnerText();
                    continue;
                }
                else if(GetTextFromAlert() == "Лайк не был поставлен" )
                {
                    DelayPayments();
                }

                perfomanse = GetElementByClassName("groups").GetInnerText();
            }
        }
        /// <summary>
        /// Убрать лайк
        /// </summary>
        /// <param name="url">URL-адрес страницы с постом</param>
        protected void TakeOffLike(string url)
        {
            OpenPageInNewTab(url);
            SwitchToLastTab();
            LikeItAndMakeRepost(string.Empty);
            CloseTab();
            SwitchToTab();
            GetElementByXPath("//*[@id='buttons']/a[1]").Click();
            Thread.Sleep(1000);
            GetElementByClassName("groups").FindElements(SearchMethod.ClassName, "group").First().Click();
            AlertAccept();
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
        /// <summary>
        /// Поставить лайк и сделать репост
        /// </summary>
        protected void LikeItAndMakeRepost(string titleTask)
        {
            SetScrollPosition(10000);

            var post = GetUrlPage().Replace("https://vk.com/wall", "post");
            var buttons = GetElementsByXPath($"//*[@id='{post}']/div/div[2]/div/div[2]/div/div[1]").First().FindElements(SearchMethod.Tag, "a");
            foreach (var item in buttons)
            {
                if (item.GetTitle() == "Нравится")
                {
                    item.Click();
                }
                else if (item.GetTitle() == "Поделиться" && titleTask == "Поставить Лайк + Рассказать друзьям")
                {
                    item.Click();

                    if (GetElementById("system_msg").Displayed)
                    {
                        RefreshPage();
                        GetElementByClassName("_share active")?.Click(); //TODO:Отладить. Похоже в разных ситуациях разные стили
                        GetElementByClassName("like_btn share _share")?.Click(); //TODO:Отладить. Похоже в разных ситуациях разные стили
                    }

                    GetElementById("like_share_my").Click();
                    GetElementById("like_share_send").Click();
                    return;
                }
            }
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
                        Thread.Sleep(1500);
                        break;
                    }
                }

                CloseTab();
                SwitchToTab();
                DelayPayments();

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
                        Thread.Sleep(1500);
                        break;
                    }
                }

                CloseTab();
                SwitchToTab();
                DelayPayments();

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
