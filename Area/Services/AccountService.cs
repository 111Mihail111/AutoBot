using AutoBot.Area.Enums;
using AutoBot.Models;
using System.Collections.Generic;
using System.Linq;

namespace AutoBot.Area.Services
{
    public static class AccountService
    {
        private static List<Account> _accounts = new List<Account>();

        /// <summary>
        /// Сохранить аккаунты
        /// </summary>
        /// <param name="accounts">Лист аккаунтов</param>
        public static void SaveAccounts(List<Account> accounts)
        {
            _accounts = accounts;
        }
        /// <summary>
        /// Получить аккаунты
        /// </summary>
        /// <returns>Коллекция аккаунтов</returns>
        public static IEnumerable<Account> GetAccounts()
        {
            return _accounts;
        }
        /// <summary>
        /// Удалить аккаунты
        /// </summary>
        public static void RemoveAccounts()
        {
            _accounts = new List<Account>();
        }
        /// <summary>
        /// Получить аккаунты по типу
        /// </summary>
        /// <param name="typeCrane">Тип крана</param>
        /// <returns>Коллекция аккаунтов</returns>
        public static IEnumerable<Account> GetAccountsByType(TypeCrane typeCrane)
        {
            return GetAccounts().Where(w => w.TypeWebSite == typeCrane.ToString());
        }
        /// <summary>
        /// Получить аккаунты по типу
        /// </summary>
        /// <param name="typeEtc">Тип "прочее"</param>
        /// <returns>Коллекция аккаунтов</returns>
        public static IEnumerable<Account> GetAccountsByType(Etc typeEtc)
        {
            return GetAccounts().Where(w => w.TypeWebSite == typeEtc.ToString());
        }
        /// <summary>
        /// Получить аккаунты по типу
        /// </summary>
        /// <param name="typeService">Тип сервиса</param>
        /// <returns>Коллекция аккаунтов</returns>
        public static IEnumerable<Account> GetAccountsByType(TypeService typeService)
        {
            return GetAccounts().Where(w => w.TypeWebSite == typeService.ToString());
        }
    }
}
