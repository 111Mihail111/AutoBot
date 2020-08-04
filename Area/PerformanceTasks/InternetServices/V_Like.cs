using AutoBot.Area.Managers;
using AutoBot.Models;

namespace AutoBot.Area.PerformanceTasks.InternetServices
{
    public class V_Like : BrowserManager
    {
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\MoonBitcoin\\";

        public void GoTo(Service service)
        {
            Initialization(BROWSER_PROFILE_CRANE);
            GoToUrl(service.Url);

        }
    }
}
