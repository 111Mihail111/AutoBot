using AutoBot.Area.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Collections.Generic;

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
        /// <summary>
        /// Получить содержимое innerText атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetInnerText(this IWebElement webElement)
        {
            return webElement.GetAttribute("innerText");
        }
        /// <summary>
        /// Получить содержимое title атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetTitle(this IWebElement webElement)
        {
            return webElement.GetAttribute("title");
        }
        /// <summary>
        /// Получить содержимое class атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetClass(this IWebElement webElement)
        {
            return webElement.GetAttribute("class");
        }
        /// <summary>
        /// Получить содержимое name атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetName(this IWebElement webElement)
        {
            return webElement.GetAttribute("name");
        }
        /// <summary>
        /// Получить содержимое aria-label атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetAriaLabel(this IWebElement webElement)
        {
            return webElement.GetAttribute("aria-label");
        }
        /// <summary>
        /// Получить содержимое id атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetId(this IWebElement webElement)
        {
            return webElement.GetAttribute("id");
        }
        /// <summary>
        /// Получить содержимое data-id атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetDataId (this IWebElement webElement)
        {
            return webElement.GetAttribute("data-id");
        }
        /// <summary>
        /// Получить содержимое aria-pressed атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetAriaPressed(this IWebElement webElement)
        {
            return webElement.GetAttribute("aria-pressed");
        }



        /// <summary>
        /// Найти элемент
        /// </summary>
        /// <param name="webElement">Веб-элемент</param>
        /// <param name="searchMethod">Метод поиска</param>
        /// <param name="attribute">Атрибут, по которому осуществляется поиск</param>
        /// <returns>Элемент</returns>
        public static IWebElement FindElement(this IWebElement webElement, SearchMethod searchMethod, string attribute)
        {
            switch (searchMethod)
            {
                case SearchMethod.Tag:
                    return webElement.FindElement(By.TagName(attribute));
                case SearchMethod.ClassName:
                    return webElement.FindElement(By.ClassName(attribute));
                case SearchMethod.Id:
                    return webElement.FindElement(By.Id(attribute));
                case SearchMethod.XPath:
                    return webElement.FindElement(By.XPath(attribute));
                default:
                    return null;
            }
        }
        /// <summary>
        /// Найти элементы
        /// </summary>
        /// <param name="webElement">Веб-элемент</param>
        /// <param name="searchMethod">Метод поиска</param>
        /// <param name="attribute">Атрибут, по которому осуществляется поиск</param>
        /// <returns>Коллекция элементов</returns>
        public static IEnumerable<IWebElement> FindElements(this IWebElement webElement, SearchMethod searchMethod, string attribute)
        {
            switch (searchMethod)
            {
                case SearchMethod.Tag:
                    return webElement.FindElements(By.TagName(attribute));
                case SearchMethod.ClassName:
                    return webElement.FindElements(By.ClassName(attribute));
                case SearchMethod.Id:
                    return webElement.FindElements(By.Id(attribute));
                default:
                    return null;
            }
        }
    }
}
