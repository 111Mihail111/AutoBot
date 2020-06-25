using OpenQA.Selenium;

namespace AutoBot.Extentions
{
    public static class WebElementExtention
    {
        /// <summary>
        /// Получить значение
        /// </summary>
        /// <param name="webElement">Веб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetValue(this IWebElement webElement)
        {
            return webElement.GetAttribute("value");
        }
    }
}
