using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
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
            OpenPageInNewTab("https://www.reddit.com/");

            if (GetElementById("email-collection-tooltip-id") != null)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            GetElementByClassName("_1JBkpB_FOZMZ7IPr3FyNfH")
                .FindElements(SearchMethod.Tag, "a").First().Click();
            Thread.Sleep(1000);

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

            GetElementByClassName("m-full-width").Click();
            Thread.Sleep(2500);

            CloseTab();
            SwitchToTab();
        }

        /// <inheritdoc/>
        public void UpArrowForPost()
        {
            var postId = GetElementsByClassName("_1oQyIsiPHYt6nx7VOmd1sz").First().GetId();
            GetElementById($"upvote-button-{postId}").Click();
            Thread.Sleep(1500);
        }

        /// <inheritdoc/>
        public void DownArrowUnderPost()
        {
            var postId = GetElementsByClassName("_1oQyIsiPHYt6nx7VOmd1sz").First().GetId();
            GetElementById($"vote-arrows-{postId}")
                .FindElements(SearchMethod.Tag, "button").Last().Click();
            Thread.Sleep(1500);
        }

        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}
