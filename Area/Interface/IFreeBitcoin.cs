using AutoBot.Models;
using System.Threading.Tasks;

namespace AutoBot.Area.Interface
{
    public interface IFreeBitcoin
    {
        public Task<Crane> GoTo(Crane crane);
    }
}
