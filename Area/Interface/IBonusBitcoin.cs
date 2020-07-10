using AutoBot.Models;
using System.Threading.Tasks;

namespace AutoBot.Area.Interface
{
    public interface IBonusBitcoin
    {
        public Task<Crane> GoTo(Crane crane);
    }
}
