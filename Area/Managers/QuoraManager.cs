using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class QuoraManager : BrowserManager, IQuora
    {
        /// <inheritdoc/>
        public void Authorization(string login, string password)
        {
            string url = "https://www.quora.com/settings";
            OpenPageInNewTabAndSwitch(url);

            if (GetUrlPage() == url)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            var loginInput = GetElementById("email");
            if (string.IsNullOrWhiteSpace(loginInput.GetValue()))
            {
                loginInput.SendKeys(login);
            }

            var passwordInput = GetElementById("password");
            if (string.IsNullOrWhiteSpace(passwordInput.GetValue()))
            {
                passwordInput.SendKeys(password);
            }

            GetElementsByTagName("button").Where(w => w.GetInnerText() == "Login").First().ToClick(2000);

            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void Subscribe()
        {
            var bottomPanelForPost = GetElementsByCssSelector(".qu-alignItems--center.qu-flexWrap--nowrap").First();
            var buttons = bottomPanelForPost.FindElements(SearchMethod.Tag, "button");

            foreach (var item in buttons)
            {
                var buttonName = item.GetInnerText();
                if (buttonName.Contains("Following"))
                {
                    return;
                }
                else if (buttonName.Contains("Follow"))
                {
                    item.ToClick();
                    return;
                }
            }
        }
        /// <inheritdoc/>
        public void Unsubscribe()
        {
            var button = GetElementByCssSelector(".qu-alignItems--center.qu-flexWrap--nowrap .puppeteer_test_pressed");
            if (button == null)
            {
                return;
            }

            button.ToClick();
        }
        /// <inheritdoc/>
        public void LikeAnswer()
        {
            var button = GetElementByClassName("puppeteer_test_answer_upvote_button");
            if (bool.Parse(button.GetAriaPressed()) == true)
            {
                return;
            }

            button.ToClick(1500);
        }
        /// <inheritdoc/>
        public void RemoveLike()
        {
            var button = GetElementByClassName("puppeteer_test_answer_upvote_button");
            if (bool.Parse(button.GetAriaPressed()) == false)
            {
                return;
            }

            button.ToClick(1500);
        }
        /// <inheritdoc/>
        public void MakeRepost()
        {
            var bottomPostDiv = GetElementByClassName("qu-justifyContent--space-between");
            var leftButtonsPanel = bottomPostDiv.FindElement(SearchMethod.XPath, "div[1]");
            var divRepostContainer = leftButtonsPanel.FindElement(SearchMethod.XPath, "div[2]");
            
            divRepostContainer.FindElement(SearchMethod.Tag, "button").ToClick(2000);

            var modalFooterDiv = GetElementByCssSelector(".q-flex.qu-flexDirection--column.qu-alignItems--center");
            modalFooterDiv.FindElement(SearchMethod.Tag, "button").ToClick();
        }
        /// <inheritdoc/>
        public bool IsUserPageProfile()
        {
            var urlSection = GetUrlPage().Split('/')[3];
            if (urlSection == "profile")
            {
                return true;
            }

            return false;
        }
        /// <inheritdoc/>
        public bool IsPageFound()
        {
            return !GetTitlePage().Contains("Error 404 - Quora");
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }
    }
}