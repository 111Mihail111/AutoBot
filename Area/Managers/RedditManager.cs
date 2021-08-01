using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class RedditManager : BrowserManager, IReddit
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            OpenPageInNewTabAndSwitch("https://www.reddit.com/");

            if (GetElementById("email-collection-tooltip-id") != null)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            GetElementByClassName("_1JBkpB_FOZMZ7IPr3FyNfH").FindElements(SearchMethod.Tag, "a").First().ToClick(1000);

            var frame = GetElementByClassName("_25r3t_lrPF3M6zD2YkWvZU");
            SwitchToFrame(frame);

            var loginInput = GetElementById("loginUsername");
            if (string.IsNullOrWhiteSpace(loginInput.GetValue()))
            {
                loginInput.SendKeys(login);
            }

            var passwordInput = GetElementById("loginPassword");
            if (string.IsNullOrWhiteSpace(passwordInput.GetValue()))
            {
                passwordInput.SendKeys(password);
            }

            GetElementByClassName("m-full-width").ToClick(3000);

            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void UpArrowForPost()
        {
            var postId = GetElementsByClassName("_1oQyIsiPHYt6nx7VOmd1sz").First().GetId();
            GetElementById($"upvote-button-{postId}").ToClick(1500);
        }
        /// <inheritdoc/>
        public void DownArrowUnderPost()
        {
            var postId = GetElementsByClassName("_1oQyIsiPHYt6nx7VOmd1sz").First().GetId();
            GetElementById($"vote-arrows-{postId}").FindElements(SearchMethod.Tag, "button").Last().ToClick(1500);
        }
        /// <inheritdoc/>
        public void Subscribe()
        {
            var buttons = GetElementsByClassName("_2q1wcTx60QKM_bQ1Maev7b");

            IWebElement button;
            if (buttons.Count() == 0)
            {
                button = GetElementByClassName("_3VgTjAJVNNV7jzlnwY-OFY");
                if (button == null)
                {
                    return;
                }
            }
            else
            {
                button = buttons.First();
                if (button.GetInnerText() != "FOLLOW")
                {
                    return;
                }
            }

            button.ToClick(2000);
        }
        /// <inheritdoc/>
        public void Unsubscribe()
        {
            var buttons = GetElementsByClassName("_2q1wcTx60QKM_bQ1Maev7b");

            IWebElement button;
            if (buttons.Count() == 0)
            {
                button = GetElementByClassName("_2QmHYFeMADTpuXJtd36LQs");
                if (button == null)
                {
                    return;
                }
            }
            else
            {
                button = buttons.First();
                if (button.GetInnerText() != "UNFOLLOW")
                {
                    return;
                }
            }

            button.ToClick(2000);
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
