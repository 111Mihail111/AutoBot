using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class ClassmatesManager : BrowserManager, IClassmatesManager
    {
        /// <inheritdoc/>
        public void AuthorizationThroughMail(string email, string password)
        {
            string url = "https://ok.ru/feed";
            OpenPageInNewTab(url);

            if (url == GetUrlPage())
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            GetElementsByClassName("__gp").First().Click();
            SwitchToLastTab();

            var savedAccountDiv = GetElementById("profileIdentifier");
            if (savedAccountDiv != null)
            {
                int tabsCount = GetTabsCount();

                savedAccountDiv.Click();
                Thread.Sleep(1500);

                if (tabsCount > GetTabsCount())
                {
                    SwitchToLastTab();
                    CloseTab();
                    SwitchToTab();
                    return;
                }
                AuthorizationUnderSavedProfile(password);
                return;
            }

            var inputLogin = GetElementById("identifierId");
            if (string.IsNullOrWhiteSpace(inputLogin.GetValue()))
            {
                inputLogin.SendKeys(email);
            }

            GetElementById("identifierNext").FindElement(SearchMethod.Tag, "button").Click();

            var inputPassword = GetElementById("password").FindElement(SearchMethod.Tag, "input");
            if (string.IsNullOrWhiteSpace(inputPassword.GetValue()))
            {
                inputPassword.SendKeys(password);
            }

            GetElementById("passwordNext").FindElement(SearchMethod.Tag, "button").Click();
            Thread.Sleep(2000);

            SwitchToLastTab();
            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void JoinGroup()
        {
            RemoveTooltipForGroupAsync();
            RemoveTopPanel();

            var buttons = GetElementById("hook_Block_AltGroupMainMenu").FindElements(SearchMethod.Tag, "a");
            foreach (var item in buttons)
            {
                if (item.GetInnerText() == "Вступить")
                {
                    item.Click();
                    break;
                }
            }

            Thread.Sleep(2000);
        }
        /// <inheritdoc/>
        public void LeaveGroup()
        {
            RemoveTopPanel();

            var ddlCollections = GetElementsByClassName("dropdown");
            if (ddlCollections.Count() == 1)
            {
                return;
            }

            var item = ddlCollections.First();
            item.Click();
            Thread.Sleep(500);

            item.FindElements(SearchMethod.Tag, "a").First().Click();
            Thread.Sleep(1000);
        }
        /// <inheritdoc/>
        public void PutClass()
        {
            RemovePostDetails();

            var vidget = GetElementByClassName("mlr_bot")
                .FindElements(SearchMethod.ClassName, "widget_cnt").Last();

            string vigetName = vidget.FindElement(SearchMethod.ClassName, "widget_tx").GetInnerText();
            if (vigetName == "Класс!")
            {
                return;
            }

            vidget.Click();
            Thread.Sleep(1500);
        }
        /// <inheritdoc/>
        public void RemoveClass()
        {
            RemovePostDetails();

            var vidget = GetElementByClassName("mlr_bot")
                .FindElements(SearchMethod.ClassName, "widget_cnt").Last();

            string vigetName = vidget.FindElement(SearchMethod.ClassName, "widget_tx").GetInnerText();
            if (vigetName == "Класс")
            {
                return;
            }

            vidget.Click();
            Thread.Sleep(1500);
        }
        /// <inheritdoc/>
        public void MakeRepost()
        {
            RemovePostDetails();

            var bottom = GetElementByClassName("mlr_bot");
            var vidget = bottom.FindElements(SearchMethod.ClassName, "widget_cnt")
                .Where(w => w.FindElement(SearchMethod.ClassName, "widget_tx").GetInnerText() == "Поделиться")
                .FirstOrDefault();

            vidget.Click();
            Thread.Sleep(1500);

            var button = bottom.FindElement(SearchMethod.ClassName, "js-doNotHide");
            if (button == null)
            {
                return;
            }

            button.Click();
            Thread.Sleep(1500);
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
        /// <inheritdoc/>
        public bool IsBlokedContent()
        {
            return GetElementById("notifyPanel_msg")?.Displayed ?? false;
        }
        /// <inheritdoc/>
        public void AddToFriends()
        {
            GetElementsByClassName("view_lvl1").Where(w => w.GetInnerText() == "Добавить в друзья").First().ToClick(1500);
        }
        /// <inheritdoc/>
        public void RemoveToFriends()
        {

        }

        /// <summary>
        /// Удалить данные поста
        /// </summary>
        protected void RemovePostDetails()
        {
            Thread.Sleep(1500);
            ExecuteScript("document.getElementsByClassName('mlr_cnt')[0].remove();");
        }
        /// <summary>
        /// Авторизация под сохраненным профилем
        /// </summary>
        /// <param name="password">Пароль</param>
        protected void AuthorizationUnderSavedProfile(string password)
        {
            var inputPass = GetElementById("password").FindElement(SearchMethod.Tag, "input");
            if (string.IsNullOrWhiteSpace(inputPass.GetValue()))
            {
                inputPass.SendKeys(password);
            }

            GetElementById("passwordNext").FindElement(SearchMethod.Tag, "button").Click();
            Thread.Sleep(2000);

            CloseTab();
            SwitchToTab();
        }
        /// <summary>
        /// Удалить подсказку для группы
        /// </summary>
        protected async void RemoveTooltipForGroupAsync()
        {
            await ExecuteScriptAsync("document.getElementsByClassName('iblock-cloud')[0]?.remove()");
        }
        /// <summary>
        /// Удалить верхнюю панель
        /// </summary>
        protected void RemoveTopPanel()
        {
            ExecuteScript("document.getElementById('topPanel')?.remove();");
        }
    }
}
