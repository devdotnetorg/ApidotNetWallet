using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApidotNetWallet.Helper
{
    public static class BaseHelper
    {
        public static string GetSHA1(string inStr)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var bytePassword = Encoding.ASCII.GetBytes(inStr);
            var strsha1Password = Encoding.Default.GetString(sha1.ComputeHash(bytePassword));
            return strsha1Password;
        }
    }
}
