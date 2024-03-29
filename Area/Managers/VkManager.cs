﻿using AutoBot.Area.Enums;
using AutoBot.Area.Managers.Interface;
using AutoBot.Extentions;
using OpenQA.Selenium.Chrome;
using System.Linq;

namespace AutoBot.Area.Managers
{
    public class VkManager : BrowserManager, IVkManager
    {
        /// <inheritdoc/>
        public void JoinToComunity()
        {
            RemoveWindowMessage();
            SetScrollPosition();

            var joinButton = GetElementById("join_button");
            if (joinButton != null)
            {
                joinButton.ToClick(1500);
                return;
            }

            var publicSubscribe = GetElementById("public_subscribe");
            if (publicSubscribe != null)
            {
                publicSubscribe.ToClick(1500);
            }
        }
        /// <inheritdoc/>
        public void UnsubscribeToComunity()
        {
            RemoveWindowMessage();

            GetElementById("page_actions_btn").ToClick();
            GetElementsByClassName("page_actions_inner").First().FindElements(SearchMethod.Tag, "a").First().ToClick(1500);
        }
        /// <inheritdoc/>
        public void PutLike()
        {
            RemovePostDetails();
            RemoveWindowMessage();

            var button = GetElementByClassName("like_btns").FindElements(SearchMethod.Tag, "a");
            button.Where(w => w.GetTitle() == "Нравится").FirstOrDefault().ToClick(2000);
        }
        /// <inheritdoc/>
        public void MakeRepost()
        {
            RemovePostDetails();
            RemoveWindowMessage(true);

            var buttons = GetElementByClassName("like_btns").FindElements(SearchMethod.Tag, "a");
            buttons.Where(w => w.GetInnerText().Contains("Поделиться")).FirstOrDefault().ToClick();

            GetElementById("like_share_my").ToClick();
            GetElementById("like_share_send").ToClick(1500);
        }
        /// <inheritdoc/>
        public void RemoveLike()
        {
            PutLike();
        }
        /// <inheritdoc/>
        public bool IsBlockedCommunity()
        {
            return GetElementByXPath("/html/body/div").GetInnerText()
                .Contains("Данный материал заблокирован на территории Российской Федерации");
        }
        /// <inheritdoc/>
        public bool IsPrivateGroup()
        {
            return GetTitlePage() == "Частная группа";
        }
        /// <inheritdoc/>
        public bool IsPostFound()
        {
            string materialBlockText = GetElementByXPath("/html/body/div").GetInnerText();
            return GetTitlePage() != "Ошибка" && !materialBlockText.Contains("Данный материал заблокирован на");
        }
        /// <inheritdoc/>
        public void ToTellAboutGroup()
        {
            GetElementById("page_menu_group_share").ToClick();
            GetElementById("like_share_send").ToClick(1500);
        }
        /// <inheritdoc/>
        public void AddToFriends()
        {
            GetElementByClassName("button_wide").ToClick(1500);
        }
        /// <inheritdoc/>
        public void RemoveFriend()
        {
            GetElementByClassName("button_wide").ToClick(1500);

            var hyperLinkCollection = GetElementById("friend_status").FindElements(SearchMethod.Tag, "a");
            foreach (var item in hyperLinkCollection)
            {
                var textButton = item.GetInnerText();
                if (textButton == "Отписаться" || textButton == "Удалить из друзей" || textButton == "Отменить заявку")
                {
                    item.ToClick();
                    return;
                }
            }
        }
        /// <inheritdoc/>
        public bool IsBlockedAccount()
        {
            return GetElementsByClassName("profile_blocked").Any();
        }
        /// <inheritdoc/>
        public void Authorization(string loginVK, string passwordVK)
        {
            OpenPageInNewTabAndSwitch("https://vk.com/");

            if (GetTitlePage().Contains("Новости"))
            {
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

            GetElementById("index_login_button").ToClick(3000);
        }
        /// <inheritdoc/>
        public void SetContextBrowserManager(ChromeDriver chromeDriver)
        {
            SetDriver(chromeDriver);
        }


        /// <summary>
        /// Удалить данные поста
        /// </summary>
        protected void RemovePostDetails()
        {
            ExecuteScript("document.getElementsByClassName('wall_text')[0]?.remove();");
        }
        /// <summary>
        /// Удаление модальных окон vk
        /// </summary>
        protected void RemoveWindowMessage(bool isRepost = false)
        {
            string removeDivScript = string.Empty;

            if (!isRepost)
            {
                removeDivScript = "document.getElementById('box_layer_wrap')?.remove();";
            }

            ExecuteScript("document.getElementById('box_layer_bg')?.remove();" +
                "document.getElementById('stl_left')?.remove();" +
                $"{removeDivScript}" +
                "document.getElementsByClassName('popup_box_container')[0]?.remove();");
        }
    }
}