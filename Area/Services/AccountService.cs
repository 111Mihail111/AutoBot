using AutoBot.Area.Enums;
using AutoBot.Models;
using System.Collections.Generic;
using System.Linq;

namespace AutoBot.Area.Services
{
    public static class AccountService
    {
        private static List<Account> _accounts = new List<Account>();

        public static void Save(List<Account> accounts)
        {
            _accounts = accounts;
        }

        public static IEnumerable<Account> GetAccounts()
        {
            return _accounts;
        }

        public static void RemoveAccounts()
        {
            _accounts = new List<Account>();
        }

        public static IEnumerable<Account> GetAccount(TypeCrane typeCrane)
        {
            return GetAccounts().Where(w => w.TypeWebSite == typeCrane.ToString());
        }

        public static IEnumerable<Account> GetAccount(Etc typeEtc)
        {
            return GetAccounts().Where(w => w.TypeWebSite == typeEtc.ToString());
        }

        public static IEnumerable<Account> GetAccount(TypeService typeService)
        {
            return GetAccounts().Where(w => w.TypeWebSite == typeService.ToString());
        }
    }
}
