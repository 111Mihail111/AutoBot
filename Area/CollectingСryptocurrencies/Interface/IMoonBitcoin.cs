using AutoBot.Models;
using System.Threading.Tasks;

namespace AutoBot.Area.CollectingСryptocurrencies.Interface
{
    public interface IMoonBitcoin
    {
        /// <summary>
        /// Начать
        /// </summary>
        /// <param name="crane">Модель крана</param>
        /// <returns>Обновленная модель крана</returns>
        public Task<Crane> Start(Crane crane);
    }
}
