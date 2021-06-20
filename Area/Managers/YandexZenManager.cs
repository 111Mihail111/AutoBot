using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class YandexZenManager : BrowserManager, IYandexZenManager
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            string url = "https://zen.yandex.ru/user/";
            OpenPageInNewTab(url);

            if (GetUrlPage().Contains(url))
            {
                CloseCurrentTabAndSwitchToAnother();
                return;
            }

            var conservedAccount = GetElementByClassName("AuthAccountListItem");
            if (conservedAccount != null)
            {
                conservedAccount.ToClick();
                AuthorizationUnderConservedAccount(password);
                return;
            }
            else if (GetElementByClassName("CurrentAccount") != null)
            {
                AuthorizationUnderConservedAccount(password);
                return;
            }

            var loginInput = GetElementById("passp-field-login");
            if (string.IsNullOrWhiteSpace(loginInput.Text))
            {
                loginInput.SendKeys(login);
            }

            GetElementByClassName("Button2_view_action").ToClick(1500);

            var passwordInput = GetElementById("passp-field-passwd");
            if (string.IsNullOrWhiteSpace(passwordInput.Text))
            {
                passwordInput.SendKeys(password);
            }

            GetElementByClassName("Button2_view_action").ToClick(2000);
            CloseCurrentTabAndSwitchToAnother();
        }

        /// <inheritdoc/>
        public void PutLike()
        {
            var panelElements = GetElementByCssSelector(".left-block-redesign-view");

            var buttons = panelElements.FindElements(SearchMethod.Tag, "button");
            foreach (var button in buttons)
            {
                var textButton = button.GetInnerText();
                if (textButton == "Нравится")
                {
                    var svgElementActive = button.FindElements(SearchMethod.Selector, "._state_active").FirstOrDefault();
                    if (svgElementActive != null)
                    {
                        return;
                    }

                    button.ToClick(1500);
                    return;
                }
            }
        }

        /// <inheritdoc/>
        public void RemoveLike()
        {
            var panelElements = GetElementByCssSelector(".left-block-redesign-view");

            var buttons = panelElements.FindElements(SearchMethod.Tag, "button");
            foreach (var button in buttons)
            {
                var textButton = button.GetInnerText();
                if (textButton == "Нравится")
                {
                    var svgElementPassive = button.FindElements(SearchMethod.Selector, "._state_passive").FirstOrDefault();
                    if (svgElementPassive != null)
                    {
                        return;
                    }

                    button.ToClick(1500);
                    return;
                }
            }
        }

        /// <inheritdoc/>
        public void Subscribe()
        {
            //Вариант подписки с одного места
            var panelElements = GetElementByClassName("publisher-controls__buttons");
            if (panelElements != null)
            {
                {
                    var buttons = panelElements.FindElements(SearchMethod.Tag, "button");
                    foreach (var button in buttons)
                    {
                        var textButton = button.GetInnerText();
                        if (textButton == "Подписаться")
                        {
                            button.ToClick(1500);
                            return;
                        }
                    }

                    return;
                }
            }

            //Вариант подписки с другого места
            panelElements = GetElementByCssSelector(".desktop-channel-3-social-layout");
            {
                var buttons = panelElements.FindElements(SearchMethod.Tag, "button");
                foreach (var button in buttons)
                {
                    var textButton = button.GetInnerText();
                    if (textButton == "Подписаться")
                    {
                        button.ToClick(1500);
                        return;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Unsubscribe()
        {
            //Вариант отписки с одного места
            var panelElements = GetElementByClassName("publisher-controls__buttons");
            if (panelElements != null)
            {
                {
                    var buttons = panelElements.FindElements(SearchMethod.Tag, "button");
                    foreach (var button in buttons)
                    {
                        var textButton = button.GetInnerText();
                        if (textButton == "Вы подписаны")
                        {
                            button.ToClick(1500);
                            return;
                        }
                    }

                    return;
                }
            }

            //Вариант отписки с другого места
            panelElements = GetElementByCssSelector(".desktop-channel-3-social-layout");
            {
                var buttons = panelElements.FindElements(SearchMethod.Tag, "button");
                foreach (var button in buttons)
                {
                    var textButton = button.GetInnerText();
                    if (textButton == "Вы подписаны")
                    {
                        button.ToClick(1500);
                        return;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }

        /// <summary>
        /// Авторизация под сохраненным аккаунтом
        /// </summary>
        /// <param name="password">Пароль к аккаунту</param>
        protected void AuthorizationUnderConservedAccount(string password)
        {
            var passwordInputInSaveAccount = GetElementById("passp-field-passwd");
            if (string.IsNullOrWhiteSpace(passwordInputInSaveAccount.Text))
            {
                passwordInputInSaveAccount.SendKeys(password);
            }

            GetElementByClassName("Button2_view_action").ToClick(2000);
            CloseCurrentTabAndSwitchToAnother();
        }
    }
}
