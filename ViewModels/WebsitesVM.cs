using AutoBot.Models;
using System.Collections.Generic;

namespace AutoBot.ViewModels
{
    /// <summary>
    /// Вэб-сайт VM
    /// </summary>
    public class WebsitesVM
    {
        /// <summary>
        /// Лист интернет-сервисов
        /// </summary>
        public List<InternetService> InternetServices { get; set; } //TODO: Для InternetService нужна отдельная VM
        /// <summary>
        /// Лист кранов
        /// </summary>
        public List<Crane> Cranes { get; set; } //TODO: Для Crane нужна отдельная VM
    }
}
