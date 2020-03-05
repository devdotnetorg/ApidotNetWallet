using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ApidotNetWallet.Helper
{
    public static class BaseHelper
    {
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
            //
            return hashed;
        }
        public class AppSettings
        {
            public string Secret { get; set; }
        }

        public static decimal BuySell(decimal rateBuy, decimal amountBuy, decimal rateSell)
        {
            var sumBuy = amountBuy / rateBuy;
            var sumSell = sumBuy * rateSell;
            sumSell = Math.Round(sumSell, 2);
            return sumSell;
        }
    }
}
