using AutoBot.Models;
using OpenQA.Selenium.Chrome;

namespace AutoBot.Area.PerformanceTasks.Interface
{
    public interface IVkTarget
    {
        public void GoTo(string url);

        public void Quit();

    }
}
