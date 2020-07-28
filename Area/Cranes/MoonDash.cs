using AutoBot.Area.Interface;
using AutoBot.Area.Managers;
using AutoBot.Models;
using System.Threading.Tasks;

namespace AutoBot.Area.Cranes
{
    public class MoonDash : BrowserManager, IMoonDash
    {
        private IRuCaptchaController _ruCaptchaController;
        const string LOGIN = "polowinckin.mixail@yandex.ru"; //TODO: Настройки вынести отдельно на страницу
        const string BROWSER_PROFILE_CRANE = "C:\\_VS_Project\\Mihail\\AutoBot\\BrowserSettings\\Profiles\\MoonDash\\";
        private string _errorZeroBalance;

        public Task<Crane> Start(Crane crane)
        {
            throw new System.NotImplementedException();
        }
    }
}
