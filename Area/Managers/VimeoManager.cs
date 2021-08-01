using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class VimeoManager : BrowserManager, IVimeoManager
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            OpenPageInNewTabAndSwitch("https://vimeo.com/");

            if (GetElementById("topnav_menu_avatar") != null)
            {
                return;
            }

            GetElementByClassName("js-topnav_menu_auth").ToClick();

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
        public void Subscribe()
        {
            var divPanel = GetElementByCssSelector(".InfoCard-sc-1l1ehfe-0.kJlqmZ");
            var button = divPanel.FindElement(SearchMethod.Selector, ".CommonStyles__FollowButton-sc-1w85dbj-7.ijjoJn");

            if (button.FindElements(SearchMethod.Tag, "svg").Count() == 2)
            {
                return;
            }
            
            button.ToClick(1700);
        }
        /// <inheritdoc/>
        public void Unsubscribe()
        {
            var divPanel = GetElementByCssSelector(".InfoCard-sc-1l1ehfe-0.kJlqmZ");
            var button = divPanel.FindElement(SearchMethod.Selector, ".CommonStyles__FollowButton-sc-1w85dbj-7.ijjoJn");

            if (button.FindElements(SearchMethod.Tag, "svg").Count() != 2)
            {
                return;
            }

            button.ToClick(1700);
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
