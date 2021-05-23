using AutoBot.Models;
using System.Collections.Generic;

namespace AutoBot.ViewModels
{
    /// <summary>
    /// Вэб-сайт VM
    /// </summary>
    public class WebSitesVM
    {
        /// <summary>
        /// Лист интернет-сервисов
        /// </summary>
        public List<InternetService> InternetServices { get; set; } //TODO: Для InternetService нужна отдельная VM
    }
}
