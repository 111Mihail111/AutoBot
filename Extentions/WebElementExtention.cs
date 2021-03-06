﻿using AutoBot.Area.Enums;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Threading;

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
        /// Получить содержимое fill атрибута
        /// </summary>
        /// <param name="webElement">Вэб-элемент</param>
        /// <returns>Значение атрибута</returns>
        public static string GetFill(this IWebElement webElement)
        {
            return webElement.GetAttribute("fill");
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
            return searchMethod switch
            {
                SearchMethod.Tag => webElement.FindElement(By.TagName(attribute)),
                SearchMethod.ClassName => webElement.FindElement(By.ClassName(attribute)),
                SearchMethod.Id => webElement.FindElement(By.Id(attribute)),
                SearchMethod.XPath => webElement.FindElement(By.XPath(attribute)),
                SearchMethod.Selector => webElement.FindElement(By.CssSelector(attribute)),
                _ => null,
            };
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
            return searchMethod switch
            {
                SearchMethod.Tag => webElement.FindElements(By.TagName(attribute)),
                SearchMethod.ClassName => webElement.FindElements(By.ClassName(attribute)),
                SearchMethod.Id => webElement.FindElements(By.Id(attribute)),
                SearchMethod.XPath => webElement.FindElements(By.XPath(attribute)),
                SearchMethod.Selector => webElement.FindElements(By.CssSelector(attribute)),
                _ => null,
            };
        }


        /// <summary>
        /// Нажать на элемент
        /// </summary>
        /// <param name="webElement">Веб-элемент</param>
        /// <param name="timeout">Время ожидания после нажатия</param>
        public static void ToClick(this IWebElement webElement, int timeout = 1000)
        {
            if (webElement == null)
            {
                throw new NullReferenceException();
            }
            else if (!webElement.Displayed)
            {
                throw new ElementNotVisibleException();
            }

            webElement.Click();
            Thread.Sleep(timeout);
        }
    }
}
