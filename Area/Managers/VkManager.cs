using System.Threading;

namespace AutoBot.Area.Managers
{
    public class VkManager : BrowserManager
    {

        public async void JoinGroup()
        {

        }

        /// <summary>
        /// Частная группа
        /// </summary>
        public bool IsPrivateGroup()
        {
            if (GetTitlePage() == "Частная группа")
            {
                return true;
            }

            return false;
        }
    }
}
