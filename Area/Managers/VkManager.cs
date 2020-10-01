﻿using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Threading;

namespace AutoBot.Area.Managers
{
    public class VkManager : BrowserManager, IVkManager
    {
        public void JoinToComunity()
        {
            RemoveWindowMessage();

            var joinButton = GetElementById("join_button");
            if (joinButton != null)
            {
                joinButton.Click();
                Thread.Sleep(1500);
                return;
            }

            var publicSubscribe = GetElementById("public_subscribe");
            if (publicSubscribe != null)
            {
                publicSubscribe.Click();
                Thread.Sleep(1500);
            }
        }

        public void UnsubscribeToComunity()
        {
            RemoveWindowMessage();

            GetElementById("page_actions_btn").Click();
            Thread.Sleep(400);

            GetElementsByClassName("page_actions_inner").First().FindElements(SearchMethod.Tag, "a").First().Click();
            Thread.Sleep(1500);
        }

        public void PutLike()
        {
            RemoveWindowMessage();

            var button = GetElementByClassName("like_btns").FindElements(SearchMethod.Tag, "a");
            int scrollTop = 100;
            bool isBlocked = true;

            while (isBlocked)
            {
                try
                {
                    button.Where(w => w.GetTitle() == "Нравится").FirstOrDefault().Click();
                    isBlocked = false;
                }
                catch
                {
                    SetScrollPosition(scrollTop);
                    scrollTop += 50;
                }
            }
        }

        public void MakeRepost()
        {
            RefreshPage();
            RemoveWindowMessage();

            var button = GetElementByClassName("like_btns").FindElements(SearchMethod.Tag, "a");
            int scrollTop = 100;
            bool isBlocked = true;

            while (isBlocked)
            {
                try
                {
                    button.Where(w => w.GetTitle() == "Поделиться").FirstOrDefault().Click();

                    GetElementById("like_share_my").Click();
                    GetElementById("like_share_send").Click();
                    Thread.Sleep(1000);
                    isBlocked = false;
                }
                catch
                {
                    SetScrollPosition(scrollTop);
                    scrollTop += 50;
                }
            }
        }

        public void RemoveLike(string url)
        {
            RemoveWindowMessage();
            OpenPageInNewTab(url);
            SwitchToLastTab();
            PutLike();
        }

        public bool IsBlockedCommunity()
        {
            return GetElementByXPath("/html/body/div").GetInnerText()
                .Contains("Данный материал заблокирован на территории Российской Федерации");
        }

        public bool IsPrivateGroup()
        {
            return GetTitlePage() == "Частная группа";
        }

        public void Authorization(string loginVK, string passwordVK)
        {
            OpenPageInNewTab("https://vk.com/");

            if (GetElementById("index_login") == null)
            {
                CloseTab();
                SwitchToTab();
                return;
            }

            var login = GetElementById("index_email");
            if (string.IsNullOrWhiteSpace(login.Text))
            {
                login.SendKeys(loginVK);
            }

            var password = GetElementById("index_pass");
            if (string.IsNullOrWhiteSpace(password.Text))
            {
                password.SendKeys(passwordVK);
            }

            GetElementById("index_login_button").Click();
            Thread.Sleep(3000);

            CloseTab();
            SwitchToTab();
        }

        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }

        /// <summary>
        /// Удаление модальных окон vk
        /// </summary>
        protected void RemoveWindowMessage()
        {
            ExecuteScript("document.querySelector('#box_layer_bg').remove();");
            ExecuteScript("document.querySelector('#stl_left').remove();");
            ExecuteScript("document.querySelector('#box_layer_wrap').remove();");
        }
    }
}