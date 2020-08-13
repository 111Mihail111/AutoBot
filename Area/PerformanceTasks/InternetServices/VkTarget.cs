using AutoBot.Area.Enums;
using AutoBot.Area.Managers;
using AutoBot.Area.Services;
using AutoBot.Models;
using System.Linq;
using System.Net;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class VkTarget : BrowserManager
    {
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\PerformanceTasks\\V_Like\\";
        private string _login;
        private string _password;
        private string _loginVK;
        private string _passwordVK;
        private string _loginTwiter;
        private string _passwordTwiter;

        protected void Init()
        {
            var accounts = AccountService.GetAccount(TypeService.VkTarget);

            var accountVK = accounts.First();
            _loginVK = accountVK.Login;
            _passwordVK = accountVK.Password;

            var accountInstagram = accounts.Last();
            _loginTwiter = accountInstagram.Login;
            _passwordTwiter = accountInstagram.Password;

            Initialization(BROWSER_PROFILE_CRANE);
        }

        public InternetService GoTo(InternetService service)
        {
            Init();
            GoToUrl(service.URL);
            Authorization(service.URL);

            return null;
        }

        /// <summary>
        /// Начать сбор
        /// </summary>
        public void BeginCollecting()
        {
            if (!CheckIsTask())
            {
                return;
            }
            //*[@id="list"]/main/section[2]/div/div/div/div[1]/div[2]/div[1]/div[3]/div/a
        }
        /// <summary>
        /// Проверить задания
        /// </summary>
        /// <returns>True - заданий нет, иначе false</returns>
        public bool CheckIsTask()
        {
            var messageNoTask = GetElementByXPath("//*[@id='list']/main/section[2]/div/div/div/div[1]/div[1]/div");
            if (messageNoTask.Displayed)
            {
                return true;
            }

            return false;
        }

        public void Authorization(string url)
        {
            if (GetUrlPage() == url)
            {
                return;
            }

            GetElementById("auth_email").SendKeys(_login);
            GetElementByXPath("//*[@id='login_form']/div[4]/div[2]/input").SendKeys(_password);
            GetElementByXPath("//*[@id='login_form']/div[7]/div[1]/button").Click();
        }
    }
}
