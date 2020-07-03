using OpenQA.Selenium;

namespace AutoBot.Extentions
{
    public static class WebElementExtention
    {
        /// <summary>
        /// Получить содержимое value атрибута
        /// </summary>
        /// <param name="webElement">Веб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetValue(this IWebElement webElement)
        {
            return webElement.GetAttribute("value");
        }

        /// <summary>
        /// Получить содержимое src атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetSrc(this IWebElement webElement)
        {
            return webElement.GetAttribute("src");
        }

        /// <summary>
        /// Получить содержимое data-sitekey атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetDataSitekey(this IWebElement webElement)
        {
            return webElement.GetAttribute("data-sitekey");
        }

        /// <summary>
        /// Получить содержимое data-fv-addons-recaptcha2-sitekey атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetDataFvAddonsRecaptcha2Sitekey(this IWebElement webElement)
        {
            return webElement.GetAttribute("data-fv-addons-recaptcha2-sitekey");
        }
    }
}
