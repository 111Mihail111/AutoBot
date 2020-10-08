namespace AutoBot.Area.Enums
{
    /// <summary>
    /// Действие в браузере
    /// </summary>
    public enum ActionToBrowser
    {
        /// <summary>
        /// Фокус на элемент
        /// </summary>
        FocusOnElement = 0,
        /// <summary>
        /// Фокус на элементы
        /// </summary>
        FocusOnElements = 1,
        /// <summary>
        /// Фокус на страницу
        /// </summary>
        FocusOnTab = 2,
        /// <summary>
        /// Обновить страницу
        /// </summary>
        RefreshPage = 3,
        /// <summary>
        /// Бездействие
        /// </summary>
        Inaction = 4,
    }
}
