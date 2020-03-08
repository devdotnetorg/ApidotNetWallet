using System;
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
