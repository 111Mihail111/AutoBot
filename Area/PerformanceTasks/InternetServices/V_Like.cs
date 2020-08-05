using AutoBot.Area.Managers;
using AutoBot.Area.PerformanceTasks.Interface;
using AutoBot.Models;
using System.Threading;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class V_Like : BrowserManager, IV_Like
    {
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\PerformanceTasks\\V_Like\\";
        const string LOGIN = "89524349046"; //TODO: Настройки вынести отдельно на страницу
        const string PASSWORD = "123qwerzxcv";  //TODO: Настройки вынести отдельно на страницу

        public InternetService GoTo(InternetService service)
        {
            Initialization(BROWSER_PROFILE_CRANE);
            GoToUrl(service.URL);
            Authorization();

            return null;
        }

        private void Authorization()
        {
            GetElementByXPath("//*[@id='uLogin']/div").Click();
            Thread.Sleep(3000);

            if (GetTabsCount() > 1)
            {
                SwitchToLastTab();
                InsertLoginAndPassword();
            }
        }

        public void InsertLoginAndPassword() //TODO:Метод будет асинхронным
        {
            GetElementByXPath("//*[@id='login_submit']/div/div/input[6]").SendKeys(LOGIN); //TODO:Создать 2 новых метода. Второй асинхронный
            GetElementByXPath("//*[@id='login_submit']/div/div/input[7]").SendKeys(PASSWORD); //TODO:Создать 2 новых метода. Второй асинхронный
            GetElementById("install_allow").Click();
        }
    }
}
