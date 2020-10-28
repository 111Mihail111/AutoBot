using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class SoundCloudManager : BrowserManager, ISoundCloud
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            string url = "https://soundcloud.com/";
            OpenPageInNewTab(url);

            if (GetUrlPage() != url)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            GetElementByClassName("frontHero__loginButton").Click();
            Thread.Sleep(1500);

            var frame = GetElementByClassName("webAuthContainer__iframe");
            SwitchToFrame(frame);

            var inputLogin = GetElementById("sign_in_up_email");
            if (string.IsNullOrEmpty(inputLogin.GetValue()))
            {
                inputLogin.SendKeys(login);
            }

            GetElementById("sign_in_up_submit").Click();
            Thread.Sleep(2500);

            SwitchToDefaultContent();

            frame = GetElementByClassName("webAuthContainer__iframe");
            SwitchToFrame(frame);

            var inputPassword = GetElementById("enter_password_field");
            if (string.IsNullOrEmpty(inputPassword.GetValue()))
            {
                inputPassword.SendKeys(password);
            }

            GetElementById("enter_password_submit").Click();
            Thread.Sleep(2500);

            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void Subscribe()
        {
            var buttons = GetElementsByClassName("sc-button-cta");
            if (buttons.Count() <= 1)
            {
                return;
            }

            foreach (var button in buttons)
            {
                if (button.GetAriaLabel() == "Follow")
                {
                    button.Click();
                    Thread.Sleep(1500);
                    break;
                }
            }
        }
        /// <inheritdoc/>
        public void Unsubscribe()
        {
            var buttons = GetElementsByClassName("sc-button-selected");
            if (buttons.Count() <= 1)
            {
                return;
            }

            foreach (var button in buttons)
            {
                if (button.GetAriaLabel() == "Unfollow")
                {
                    button.Click();
                    Thread.Sleep(1500);
                    break;
                }
            }
        }
        /// <inheritdoc/>
        public void LikeTrack()
        {
            var buttons = GetElementsByClassName("sc-button-like");
            foreach (var button in buttons)
            {
                string attribute = button.GetAriaLabel();
                if (attribute == "Like")
                {
                    button.Click();
                    Thread.Sleep(1500);
                    return;
                }
                else if (attribute  == "Unlike")
                {
                    return;
                }
            }
        }
        /// <inheritdoc/>
        public void RemoveLike()
        {
            var buttons = GetElementsByClassName("sc-button-like");
            foreach (var button in buttons)
            {
                string attribute = button.GetAriaLabel();
                if (attribute == "Unlike")
                {
                    button.Click();
                    Thread.Sleep(1500);
                    return;
                }
                else if (attribute == "Like")
                {
                    return;
                }
            }
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);

        }
    }
}