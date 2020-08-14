using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Area.Services;
using AutoBot.Extentions;
using AutoBot.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Linq;
using System.Security.Policy;
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
            AddToFriendsVK();
            SubscriptionsInInstagram();
            WorkWithLikeInstagram();
        }

        /// <summary>
        /// Вступить в сообщество ВК
        /// </summary>
        protected void JoinInCommunityVK()
        {
            GetElementByXPath("//*[@id='vk1']/a").Click(); //Вступление в ВК

            var message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            while (message == null)
            {
                GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/a").Click();

                if (GetTabsCount() > 1)
                {
                    SwitchToLastTab();
                    CloseTab();
                    SwitchToTab();
                }

                GetElementByXPath("//*[@id='content']/p[4]/a").Click();
                SwitchToLastTab();

                BeautifulAddressForVK();
                LinkEmailVK();

                if (GetTitlePage() == "Частная группа")
                {
                    CloseTab();
                    SwitchToTab();
                    GetElementByXPath("//*[@id='buttons']/a[1]").Click();
                    Thread.Sleep(1000);
                    GetElementByXPath("//*[@id='content']/div[3]/div[1]/a[1]").Click();
                    continue;
                }

                GetElementById("join_button")?.Click();
                GetElementById("public_subscribe")?.Click();

                Thread.Sleep(1000);

                CloseTab();
                SwitchToTab();
                GetElementByXPath("//*[@id='buttons']/a[2]").Click();

                message = GetElementByXPath("//*[@id='content']/div[3]/div/p[1]/b");
            }

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
                var title = GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[2]/p").GetInnerText();
                SwitchToLastTab();
                Thread.Sleep(500);
                BeautifulAddressForVK();
                LinkEmailVK();

                if (IsError())
                {
                    CloseTab();
                    SwitchToTab();
                    GetElementByXPath("//*[@id='buttons']/a[1]").Click();
                    GetElementByXPath("//*[@id='content']/div[3]/div[1]/div[3]/span").Click();
                    AlertAccept();
                    continue;
                }

                LinkEmailVK();

                if (title == "Поставить Лайк + Рассказать друзьям")
                {
                    LikeIt();
                    MakeRepost();
                }
                else
                {
                    LikeIt();
                }

                string url = GetUrlPage();
                CloseTab();
                SwitchToTab();
                Thread.Sleep(5000);
                GetElementByXPath("//*[@id='buttons']/a[2]").Click();

                if (GetTextFromAlert() == "К сожалению, уже было поставлено нужное количество лайков к данном объекту. Обновите список заданий.")
                {
                    OpenPageInNewTab(url);
                    SwitchToLastTab();
                    LikeIt();
                    CloseTab();
                    SwitchToTab();
                    GetElementByXPath("//*[@id='buttons']/a[1]").Click();
                    GetElementByXPath("//*[@id='vk2']/a").Click();
                }

                Thread.Sleep(1000);
                perfomanse = GetElementByClassName("groups").GetInnerText();
            }
        }

        /// <summary>
        /// Привязка Email к аккаунту Vk
        /// </summary>
        protected void LinkEmailVK()
        {
            if (GetElementById("stl_left").Displayed)
            {
                ExecuteScript("document.querySelector('#box_layer_bg').remove();");
                ExecuteScript("document.querySelector('#box_layer_wrap').remove()");
            }
        }

        protected void BeautifulAddressForVK()
        {
            if (GetElementById("stl_left").Displayed)
            {
                ExecuteScript("document.querySelector('#stl_left').remove();");
                ExecuteScript("document.querySelector('#box_layer_wrap').remove()");
            }
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

                BeautifulAddressForVK();
                LinkEmailVK();

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

                var subscribe = GetElementByTagName("button");
                if (subscribe == null)
                {
                    CloseTab();
                    SwitchToTab();
                    GetElementByXPath("//*[@id='buttons']/a[1]").Click();
                    Thread.Sleep(1500);
                    GetElementByXPath("//*[@id='content']/div[2]/div[1]/a[1]").Click();
                    AlertAccept();
                    Thread.Sleep(2000);
                    continue;
                }
                subscribe.Click();

                var loginForm = GetElementById("loginForm");
                if (loginForm != null && loginForm.Displayed)
                {
                    AuthorizationInInstagram();
                    GetElementByTagName("button").Click();
                }

                CloseTab();
                SwitchToTab();
                Thread.Sleep(1000);

                try
                {
                    var modal = GetElementById("modal");
                    while (modal.Displayed)
                    {
                        GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                        AlertAccept();
                        Thread.Sleep(2000);
                    }
                }
                catch
                {
                    //Alert'a нет
                }

                Thread.Sleep(2000);
                groups = GetElementsByClassName("groups");
            }
        }
        protected void WorkWithLikeInstagram()
        {
            GetElementByXPath("//*[@id='in1']/a").Click();

            var groups = GetElementsByClassName("groups");
            while (groups.Count() != 0)
            {
                GetElementByXPath("//*[@id='content']/div[2]/div[1]/div[3]/a").Click();
                SwitchToLastTab();

                var subscribe = GetElementByTagName("button");
                if (subscribe == null)
                {
                    CloseTab();
                    SwitchToTab();
                    GetElementByXPath("//*[@id='buttons']/a[1]").Click();
                    Thread.Sleep(1500);
                    GetElementByXPath("//*[@id='content']/div[2]/div[1]/a[1]").Click();
                    continue;
                }

                var likeButton = GetElementByXPath("//*[@id='react-root']/section/main/div/div[1]/article/div[3]/section[1]/span[1]/button");
                likeButton.Click();

                var loginForm = GetElementById("loginForm"); //TODO: 
                if (loginForm != null && loginForm.Displayed)
                {
                    AuthorizationInInstagram();
                    GetElementByTagName("button").Click();
                    likeButton.Click();
                }

                CloseTab();
                SwitchToTab();
                Thread.Sleep(1000);

                try
                {
                    var modal = GetElementById("modal");
                    while (modal.Displayed)
                    {
                        GetElementByXPath("//*[@id='buttons']/a[2]").Click();
                        AlertAccept();
                        Thread.Sleep(3000);
                    }
                }
                catch
                {
                    //Alert'a нет
                }

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
        /// Поставить лайк
        /// </summary>
        protected void LikeIt()
        {
            var post = GetUrlPage().Replace("https://vk.com/wall", "post");
            SetScrollPosition(10000);
            GetElementByXPath($"//*[@id='{post}']/div/div[2]/div/div[2]/div/div[1]/a[1]/div[1]")?.Click();
            GetElementByXPath("//*[@id='pv_narrow']/div[1]/div[1]/div/div/div[1]/div[3]/div/div[1]/a[1]/div[1]")?.Click();
        }
        /// <summary>
        /// Сделать репост
        /// </summary>
        protected void MakeRepost()
        {
            var post = GetUrlPage().Replace("https://vk.com/wall", "post");
            SetScrollPosition(10000);

            string script = $"return document.querySelector('#{post}>div>div.post_content>div>div.like_wrap._like_wall-197684583_14').getElementsByTagName('a').length;";
            ExecuteScript(script);
            if (Convert.ToInt32(ExecuteScript(script)) == 3)
            {
                GetElementByXPath($"//*[@id='{post}']/div/div[2]/div/div[2]/div/div[1]/a[3]/div[1]").Click();
            }
            else
            {
                GetElementByXPath($"//*[@id='{post}']/div/div[2]/div/div[2]/div/div[1]/a[2]/div[1]")?.Click();
            }
            
            GetElementById("like_share_my").Click();
            GetElementById("like_share_send").Click();
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
