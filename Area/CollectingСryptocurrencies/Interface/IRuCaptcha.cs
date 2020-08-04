using System.Threading.Tasks;

namespace AutoBot.Area.CollectingСryptocurrencies.Interface
{
    public interface IRuCaptcha
    {
        /// <summary>
        /// Перейти на страницу RuCaptcha
        /// </summary>
        public Task GoTo();
    }
}
