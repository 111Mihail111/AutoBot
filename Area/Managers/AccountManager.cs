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
        public void SaveAccounts(IFormFile fileAccounts)
        {
            var lineData = GetLinesFile(ReadFile(fileAccounts));
            var accounts = GetUserAccounts(lineData);

            if (AccountService.GetAccounts().Any())
            {
                AccountService.RemoveAccounts();
            }

            AccountService.Save(accounts);
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
                    case 3:
                        if (array[0] == "RuCaptcha")
                        {
                            accounts.Add(new Account { TypeWebSite = array[0], ApiKey = array[1] });
                            continue;
                        }
                        accounts.Add(new Account { TypeWebSite = array[0], Login = array[1], AccountType = array[3].ConvertStringToEnum<AccountType>() });
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
        /// <summary>
        /// Считать содержимое файла
        /// </summary>
        /// <param name="fileAccounts">Файл аккаунтов</param>
        /// <returns>Строка данных</returns>
        protected string ReadFile(IFormFile fileAccounts)
        {
            string textFromFile;

            using (Stream stream = fileAccounts.OpenReadStream())
            {
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, array.Length);
                textFromFile = System.Text.Encoding.UTF8.GetString(array);
            }

            return textFromFile;
        }
    }
}
