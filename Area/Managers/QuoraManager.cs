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
            OpenPageInNewTab(url);

            if (GetUrlPage() == url)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            var inputCollection = GetElementsByClassName("ignore_interaction");
            foreach (var input in inputCollection)
            {
                string inputName = input.GetName();
                string inputValue = input.GetValue();

                if (inputName == "email")
                {
                    if (string.IsNullOrWhiteSpace(inputValue))
                    {
                        input.SendKeys(login);
                    }
                }
                else if (inputName == "password")
                {
                    if (string.IsNullOrWhiteSpace(inputValue))
                    {
                        input.SendKeys(password);
                    }
                }
            }

            inputCollection.Last().Click();
            Thread.Sleep(1500);

            CloseTab();
            SwitchToTab();
        }
        /// <inheritdoc/>
        public void Subscribe()
        {
            var button = GetElementByClassName("qu-justifyContent--space-between").FindElements(SearchMethod.Tag, "button").First();            
            if (button.GetInnerText().Contains("Following"))
            {
                return;
            }

            button.Click();
            Thread.Sleep(1500);
        }
        /// <inheritdoc/>
        public void Unsubscribe()
        {
            var buttonCollection = GetElementsByClassName("puppeteer_test_pressed");
            if (!buttonCollection.Any())
            {
                return;
            }

            buttonCollection.First().Click();
            Thread.Sleep(1500);
        }
        /// <inheritdoc/>
        public void LikeAnswer()
        {
            var button = GetElementByClassName("puppeteer_test_answer_upvote_button");
            if (bool.Parse(button.GetAriaPressed()) == true)
            {
                return;
            }

            button.Click();
            Thread.Sleep(1500);
        }
        /// <inheritdoc/>
        public void RemoveLike()
        {
            var button = GetElementByClassName("puppeteer_test_answer_upvote_button");
            if (bool.Parse(button.GetAriaPressed()) == false)
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