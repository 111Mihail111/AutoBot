using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class VimeoManager : BrowserManager, IVimeoManager
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            OpenPageInNewTab("https://vimeo.com/");

            if (GetElementById("topnav_menu_avatar") != null)
            {
                CloseTab();
                SwitchToTab();
                return;
            }


            GetElementById("nav-cta-login").ToClick();

            var inputLogin = GetElementById("signup_email");
            if (string.IsNullOrWhiteSpace(inputLogin.GetValue()))
            {
                inputLogin.SendKeys(login);
            }

            var inputPassword = GetElementById("login_password");
            if (string.IsNullOrWhiteSpace(inputPassword.GetValue()))
            {
                inputPassword.SendKeys(password);
            }

            GetElementByClassName("js-email-submit").ToClick(1500);

            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void LikeUnderVideo()
        {
            var button = GetElementByClassName("like-button");
            if (button.GetAriaLabel() != "Like")
            {
                return;
            }

            button.ToClick();
        }
        /// <inheritdoc/>
        public void RemoveLike()
        {
            var button = GetElementByClassName("like-button");
            if (button.GetAriaLabel() != "Unlike")
            {
                return;
            }

            button.ToClick();
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
