using AutoBot.Enums;
using AutoBot.Models;
using System.Threading.Tasks;

namespace AutoBot.Area.Interface
{
    public interface IFreeBitcoin
    {
        public Task<Crane> GoTo(Crane crane);
        public Task AuthorizationOnCrane(string urlCrane);
        public string GetDataCaptcha(Captcha captcha);
        public string GetTokenCaptcha(string url);
        public string ConvertImageToByte();
        public bool CheckPage(string url);
        public bool IsTimerExist();
        public string GetTimer();
        public double BalanceCrane();
    }
}
