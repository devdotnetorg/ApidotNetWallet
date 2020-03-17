using System;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ApidotNetWallet.Helper
{
    /// <summary>
    /// Helper
    /// </summary>
    public static class BaseHelper
    {
        /// <summary>
        /// Формирование ключа на основе пароля по стандарту PBKDF2 
        /// Создание пользователя
        /// </summary>
        /// <returns>
        /// Ключ
        /// </returns>
        /// <param name="password">Парольная фраза
        /// </param>
        public static string GetPbkdf2(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            /*
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            */
            //add static
            byte[] salt = new byte[128 / 8] { 189, 43, 133, 170, 47, 157, 231, 217, 36, 206, 213, 49, 14, 51, 80, 249 };
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        /// <summary>
        /// Проверка правильноести Email адреса
        /// </summary>
        /// <returns>
        /// True- если Email, false - если не Email
        /// </returns>
        /// <param name="email">Email
        /// </param>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Настройки приложения.
        /// </summary>
        public class AppSettings
        {
            /// <value>Ключ для шифрования токена</value>
            public string Secret { get; set; }
            public string Test { get; set; }
        }

        /// <summary>
        /// Операция покупки EUR, продажи EUR в указанную валюту.
        /// Финансовые операции проходят через EUR
        /// </summary>
        /// <returns>
        /// Итоговая сумма в кончной валюте
        /// </returns>
        /// <param name="rateBuy">Курс покупки EUR по курсу исходной валюты</param>
        /// <param name="amountBuy">Сумма в первоначальной валюте</param>
        /// <param name="rateSell">Курс продажи EUR в валюте </param>
        public static decimal BuySell(decimal rateBuy, decimal amountBuy, decimal rateSell)
        {
            var sumBuy = amountBuy / rateBuy;
            var sumSell = sumBuy * rateSell;
            sumSell = Math.Round(sumSell, 2);
            return sumSell;
        }
    }
}
