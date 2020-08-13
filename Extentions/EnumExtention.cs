using System;

namespace AutoBot.Extentions
{
    public static class EnumExtention
    {
        /// <summary>
        /// Конвертировать строку к enum'у
        /// </summary>
        /// <typeparam name="T">Обобщенный объект</typeparam>
        /// <param name="value">Значение строки</param>
        /// <returns>Объект Enum'a</returns>
        public static T ConvertStringToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
