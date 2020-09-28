﻿using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class ClassmatesManager : BrowserManager, IClassmatesManager
    {
        public void AuthorizationThroughMail (string email, string password)
        {
            string url = "https://ok.ru/feed";
            OpenPageInNewTab(url);

            if (url == GetUrlPage())
            {
                return;
            }

            GetElementsByClassName("__gp").First().Click();
            SwitchToLastTab();

            var savedAccountDiv = GetElementById("profileIdentifier");
            if (savedAccountDiv != null)
            {
                int tabsCount = GetTabsCount();
                
                savedAccountDiv.Click();
                Thread.Sleep(1500);

                if (tabsCount > GetTabsCount())
                {
                    SwitchToLastTab();
                    CloseTab();
                    SwitchToTab();
                    return;
                }
                AuthorizationUnderSavedProfile(password);
                return;
            }

            var inputLogin = GetElementById("identifierId");
            if (string.IsNullOrWhiteSpace(inputLogin.GetValue()))
            {
                inputLogin.SendKeys(email);
            }

            GetElementById("identifierNext").FindElement(SearchMethod.Tag, "button").Click();

            var inputPassword = GetElementById("password").FindElement(SearchMethod.Tag, "input");
            if (string.IsNullOrWhiteSpace(inputPassword.GetValue()))
            {
                inputPassword.SendKeys(password);
            }

            GetElementById("passwordNext").FindElement(SearchMethod.Tag, "button").Click();
            Thread.Sleep(2000);

            SwitchToTab();
            CloseTab();
            SwitchToTab();
        }

        /// <summary>
        /// Авторизация под сохраненным профилем
        /// </summary>
        /// <param name="password">Пароль</param>
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

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            GetDriver(chromeDriver);
        }
    }
}
