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

            GetElementByClassName("frontHero__loginButton").ToClick(1500);

            var frame = GetElementByClassName("webAuthContainer__iframe");
            SwitchToFrame(frame);

            ExecuteScript("document.getElementsByClassName('sc-button-google')[0].click();");
            Thread.Sleep(2500);
            SwitchToLastTab();

            var savedAccountDiv = GetElementById("profileIdentifier");
            if (savedAccountDiv != null)
            {
                savedAccountDiv.ToClick();
                AuthorizationUnderSavedProfile(password);
                return;
            }

            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void Subscribe()
        {
            RemoveModalWindow();

            var buttons = GetElementsByClassName("sc-button-cta");
            if (buttons.Count() <= 1)
            {
                return;
            }

            foreach (var button in buttons)
            {
                if (button.GetAriaLabel() == "Follow")
                {
                    button.ToClick(1500);
                    break;
                }
            }
        }
        /// <inheritdoc/>
        public void Unsubscribe()
        {
            RemoveModalWindow();

            var buttons = GetElementsByClassName("sc-button-selected");
            if (buttons.Count() <= 1)
            {
                return;
            }

            foreach (var button in buttons)
            {
                if (button.GetAriaLabel() == "Unfollow")
                {
                    button.ToClick(1500);
                    break;
                }
            }
        }
        /// <inheritdoc/>
        public void LikeTrack()
        {
            RemoveModalWindow();

            var buttons = GetElementsByClassName("sc-button-like");
            foreach (var button in buttons)
            {
                string attribute = button.GetAriaLabel();
                if (attribute == "Like")
                {
                    button.ToClick(1500);
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
            RemoveModalWindow();

            var buttons = GetElementsByClassName("sc-button-like");
            foreach (var button in buttons)
            {
                string attribute = button.GetAriaLabel();
                if (attribute == "Unlike")
                {
                    button.ToClick(1500);
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
            RemoveModalWindow();

            var button = GetElementByClassName("sc-button-repost");
            if (button.GetInnerText() != "Repost")
            {
                return;
            }

            button.ToClick(1500);
        }
        /// <inheritdoc/>
        public void RemoveRepost()
        {
            RemoveModalWindow();

            var button = GetElementByClassName("sc-button-repost");
            if (button.GetInnerText() != "Reposted")
            {
                return;
            }

            button.ToClick(1500);
        }
        /// <inheritdoc/>
        public void DownloadTrack()
        {
            RemoveModalWindow();

            var buttons = GetElementByCssSelector(".sc-button-group.sc-button-group-medium").FindElements(SearchMethod.Tag, "button");
            foreach (var button in buttons)
            {
                if (button.GetAriaLabel() == "More")
                {
                    button.ToClick(1500);
                    break;
                }
            }
            
            var buttonDownload = GetElementByClassName("sc-button-download");
            if (buttonDownload.GetAriaLabel() != "Download this track")
            {
                return;
            }

            buttonDownload.ToClick();
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }

        /// <summary>
        /// Аторизация под сохраненным профилем
        /// </summary>
        /// <param name="password">Пароль к учетной записи</param>
        protected void AuthorizationUnderSavedProfile(string password)
        {
            var inputPass = GetElementById("password").FindElement(SearchMethod.Tag, "input");
            if (string.IsNullOrWhiteSpace(inputPass.GetValue()))
            {
                inputPass.SendKeys(password);
            }

            GetElementById("passwordNext").FindElement(SearchMethod.Tag, "button").ToClick(2000);

            CloseTab();
            SwitchToTab();
        }
        protected void RemoveModalWindow()
        {
            Thread.Sleep(2000);
            ExecuteScript("document.getElementsByClassName('callout g-z-index-callout m-active')[0]?.remove();");
        }
    }
}