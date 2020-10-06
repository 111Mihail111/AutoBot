using AutoBot.Area.Enums;
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
            RemovePostDetails();
            RemoveWindowMessage();

            var button = GetElementByClassName("like_btns").FindElements(SearchMethod.Tag, "a");
            button.Where(w => w.GetTitle() == "Нравится").FirstOrDefault().Click();
        }

        public void MakeRepost()
        {
            RemovePostDetails();

            var button = GetElementByClassName("like_btns").FindElements(SearchMethod.Tag, "a");
            button.Where(w => w.GetTitle() == "Поделиться").FirstOrDefault().Click();

            GetElementById("like_share_my").Click();
            GetElementById("like_share_send").Click();
            Thread.Sleep(1500);
        }

        public void RemoveLike(string url)
        {
            RemoveWindowMessage();
            OpenPageInNewTab(url);
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

        public bool IsPostFound()
        {
            return GetTitlePage() != "Ошибка";
        }

        public void ToTellAboutGroup()
        {
            GetElementById("page_menu_group_share").Click();
            GetElementById("like_share_send").Click();
            Thread.Sleep(1500);
        }

        public void AddToFriends()
        {
            GetElementByClassName("button_wide").Click();
            Thread.Sleep(1500);
        }

        public void RemoveFromFriends() //TODO: Не отлажен
        {
            var hyperLinkCollection = GetElementsByClassName("page_actions_item");

            GetElementByClassName("button_wide").Click();
            Thread.Sleep(1500);
            foreach (var item in hyperLinkCollection)
            {
                var textButton = item.GetInnerText();
                if (textButton == "Отписаться" || textButton == "Удалить из друзей" || textButton == "Отменить заявку")
                {
                    item.Click();
                    return;
                }
            }
        }

        public bool IsBlockedAccount() //TODO: Не отлажен
        {
            return GetElementsByClassName("profile_blocked") != null;
        }

        public void Authorization(string loginVK, string passwordVK)
        {
            OpenPageInNewTab("https://vk.com/");

            if (GetTitlePage().Contains("Новости"))
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
        /// Удалить данные поста
        /// </summary>
        protected void RemovePostDetails() //Есть TODO
        {
            string postId = GetUrlPage().Replace("https://vk.com/wall", "wpt");
            ExecuteScript($"document.getElementById('{postId}')?.remove();"); //Если не null, то удаляем TODO:Придумать способ лучше
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