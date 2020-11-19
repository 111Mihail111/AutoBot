namespace AutoBot.Area.Enums
{
    /// <summary>
    /// Метод поиска
    /// </summary>
    public enum SearchMethod
    {
        /// <summary>
        /// Наименование html-тэга
        /// </summary>
        Tag = 0,
        /// <summary>
        /// Наименование класса
        /// </summary>
        ClassName = 1,
        /// <summary>
        /// Наименование id
        /// </summary>
        Id = 2,
        /// <summary>
        /// XPath путь к элементу
        /// </summary>
        XPath = 3,
    }
}
