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
        public void AuthorizationThroughMail(string login, string password)
        {
            string url = "https://ok.ru/feed";
            OpenPageInNewTab(url);

            if (url == GetUrlPage())
            {
                CloseCurrentTabAndSwitchToAnother();
                return;
            }

            var inputLogin = GetElementById("field_email");
            if (string.IsNullOrWhiteSpace(inputLogin.GetValue()))
            {
                inputLogin.SendKeys(login);
            }

            var inputPassword = GetElementById("field_password");
            if (string.IsNullOrWhiteSpace(inputPassword.GetValue()))
            {
                inputPassword.SendKeys(password);
            }

            GetElementByCssSelector(".button-pro.__wide").ToClick(1500);
            CloseCurrentTabAndSwitchToAnother();
        }
        /// <inheritdoc/>
        public void AuthorizationThroughMailOldVersionBrowser(string email, string password)
        {
            string url = "https://ok.ru/feed";
            OpenPageInNewTab(url);

            if (url == GetUrlPage())
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            var saveProfile = GetElementByClassName("anonym_login_user");
            if (saveProfile != null)
            {
                saveProfile.ToClick();

                CloseTab();
                SwitchToTab();
                return;
            }

            GetElementsByClassName("__gp").First().ToClick();
            SwitchToLastTab();

            var savedAccountLi = GetElementById($"account-{email}");
            if (savedAccountLi != null)
            {
                GetElementById("choose-account-0").ToClick(2000);
                SwitchToTab();
                return;
            }

            var inputLogin = GetElementById("Email");
            if (string.IsNullOrWhiteSpace(inputLogin.GetValue()))
            {
                inputLogin.SendKeys(email);
            }
            GetElementById("next").ToClick(2000);

            var inputPassword = GetElementById("password");
            if (string.IsNullOrWhiteSpace(inputPassword.GetValue()))
            {
                inputPassword.SendKeys(password);
            }
            GetElementById("submit").ToClick(2000);

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
                    item.ToClick();
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
            item.ToClick();

            item.FindElements(SearchMethod.Tag, "a").First().ToClick();
        }
        /// <inheritdoc/>
        public void PutClass()
        {
            RemovePostDetails();

            var vidget = GetElementById("hook_Block_ActionsPLLB");
            if (vidget == null)
            {
                vidget = GetElementByClassName("mlr_bot");
            }

            var span = vidget.FindElement(SearchMethod.Selector, ".widget_cnt.controls-list_lk");
            string vigetName = span.FindElement(SearchMethod.ClassName, "widget_tx").GetInnerText();
            
            if (vigetName == "Класс!")
            {
                return;
            }

            span.ToClick(1500);
        }
        /// <inheritdoc/>
        public void RemoveClass()
        {
            RemovePostDetails();

            var vidget = GetElementById("hook_Block_ActionsPLLB");
            if (vidget == null)
            {
                vidget = GetElementByClassName("mlr_bot");
            }

            var span = vidget.FindElement(SearchMethod.Selector, ".widget_cnt.controls-list_lk");
            string vigetName = span.FindElement(SearchMethod.ClassName, "widget_tx").GetInnerText();

            if (vigetName == "Класс")
            {
                return;
            }

            span.ToClick(1500);
        }
        /// <inheritdoc/>
        public void MakeRepost()
        {
            RemovePostDetails();

            var bottom = GetElementByClassName("mlr_bot");
            var vidget = bottom.FindElements(SearchMethod.ClassName, "widget_cnt")
                .Where(w => w.FindElement(SearchMethod.ClassName, "widget_tx").GetInnerText() == "Поделиться")
                .FirstOrDefault();

            vidget.ToClick(1500);

            var button = bottom.FindElement(SearchMethod.ClassName, "js-doNotHide");
            if (button == null)
            {
                return;
            }

            button.ToClick(1500);
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
            var panelMenuElements = GetElementById("hook_Block_MainMenu").FindElements(SearchMethod.Tag, "a");
            foreach (var element in panelMenuElements)
            {
                var elementText = element.GetInnerText();
                if (elementText != "Добавить в друзья" || elementText != "Подписаться")
                {
                    continue;
                }

                element.ToClick(1500);
                return;
            }
        }
        /// <inheritdoc/>
        public void RemoveToFriends()
        {
            var panelMenuElements = GetElementById("hook_Block_MainMenu");

            var requestSentElement = panelMenuElements.FindElements(SearchMethod.Tag, "span")
                .Where(w => w.GetInnerText() == "Запрос отправлен" || w.GetInnerText() == "Подписаны").FirstOrDefault();

            if (requestSentElement != null)
            {
                requestSentElement.ToClick(1500);

                var text = requestSentElement.GetInnerText();
                if (text == "Подписаны")
                {
                    panelMenuElements.FindElement(SearchMethod.XPath, "div/ul/li[2]/div/div/ul/li/a").ToClick(1500);
                    return;
                }

                panelMenuElements.FindElement(SearchMethod.XPath, "div/ul/li[1]/div/div/ul/li/a").ToClick(1500);
                return;
            }

            panelMenuElements.FindElement(SearchMethod.Selector, "li.u-menu_li.expand-action-item").ToClick(1500);
            panelMenuElements.FindElement(SearchMethod.XPath, "div/ul/li[4]/div/div/ul/li[8]/a").ToClick(1500);
            GetElementById("hook_FormButton_button_delete_confirm").ToClick(1500);
        }


        /// <summary>
        /// Удалить детали аккаунта в старой версии браузера
        /// </summary>
        /// <param name="login">Логин</param>
        protected void RemoveAccountDetailsOldVersionBrowser(string login)
        {
            var savedAccountLi = GetElementById($"account-{login}");
            if (savedAccountLi != null)
            {
                GetElementById("choose-account-0").ToClick(1000);
            }
        }
        /// <summary>
        /// Удалить данные поста
        /// </summary>
        protected void RemovePostDetails()
        {
            Thread.Sleep(1500);
            ExecuteScript("document.getElementsByClassName('mlr_cnt')[0]?.remove();");
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
