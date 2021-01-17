using AutoBot.Area.Enums;
using AutoBot.Area.Services;
using AutoBot.Extentions;
using AutoBot.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoBot.Area.Managers
{
    public class AccountManager
    {
        /// <summary>
        /// Сохранить аккаунты
        /// </summary>
        /// <param name="fileLine">Строка файла</param>
        public void SaveAccounts(string fileLine)
        {
            var lineData = GetLinesFile(fileLine);
            var accounts = GetUserAccounts(lineData);

            if (AccountService.GetAccounts().Any())
            {
                AccountService.RemoveAccounts();
            }

            AccountService.SaveAccounts(accounts);
        }
        /// <summary>
        /// Получить аккаунты пользователя
        /// </summary>
        /// <param name="lineData">Строка данных</param>
        /// <returns>Лист аккаунтов</returns>
        protected List<Account> GetUserAccounts(List<string> dataLines)
        {
            List<Account> accounts = new List<Account>();

            foreach (var item in dataLines)
            {
                var array = item.Split("||");
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].Replace("||", string.Empty);
                }
                switch (array.Length)
                {
                    case 2:
                    case 3:
                        if (array.Length == 2 && array[0] == "RuCaptcha")
                        {
                            accounts.Add(new Account { TypeWebSite = array[0], ApiKey = array[1] });
                            continue;
                        }
                        accounts.Add(new Account { TypeWebSite = array[0], Login = array[1], AccountType = array[2].ConvertStringToEnum<AccountType>() });
                        break;
                    default:
                        accounts.Add(new Account { TypeWebSite = array[0], Login = array[1], Password = array[2], AccountType = array[3].ConvertStringToEnum<AccountType>() });
                        break;
                }
                
            }

            return accounts;
        }
        /// <summary>
        /// Получить строки данных
        /// </summary>
        /// <param name="data">Строка данных</param>
        /// <returns></returns>
        protected List<string> GetLinesFile(string data)
        {
            var array = data.Split("|||");
            List<string> lines = new List<string>();

            foreach (var item in array)
            {
                lines.Add(item.Replace("|||", string.Empty).Replace("\r\n", string.Empty));
            }

            return lines;
        }
    }
}
