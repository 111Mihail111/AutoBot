using AutoBot.Area.Enums;

namespace AutoBot.Area.PerformanceTasks.Interface
{
    public interface IVkTarget
    {
        public void GoTo(string url, TypeService typeService);

        public void Quit();

    }
}
