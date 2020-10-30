using AutoBot.Area.Enums;
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

            ExecuteScript("document.getElementsByClassName('sc-button-google')[0].click();");
            Thread.Sleep(2500);
            SwitchToLastTab();

            var savedAccountDiv = GetElementById("profileIdentifier");
            if (savedAccountDiv != null)
            {
                savedAccountDiv.Click();
                AuthorizationUnderSavedProfile(password);
                return;
            }

            CloseTab();
            SwitchToTab();
        }
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
                else if (attribute == "Unlike")
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
        public void RepostTrack()
        {
            var button = GetElementByClassName("sc-button-repost");
            if (button.GetInnerText() != "Repost")
            {
                return;
            }

            button.Click();
            Thread.Sleep(1500);
        }
        /// <inheritdoc/>
        public void RemoveRepost()
        {
            var button = GetElementByClassName("sc-button-repost");
            if (button.GetInnerText() != "Reposted")
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
    }
}