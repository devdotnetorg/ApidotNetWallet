using System;
using System.Linq;

namespace ApidotNetWallet.Models
{
    public static class DBInitializerdotNetWallet
    {
        /// <summary>
        /// Заполнение БД. Справочники.
        /// </summary>
        public static void Seed(WalletApiContext db)
        {
            //Проверка наличие справочника
            if (!db.Currencies.Any())
            {
                //Заполнение справочника
                //RUB Рубль
                //USD Доллар
                //EUR Евро
                //IDR Индонезийская рупия
                //
                db.Currencies.Add(new Currency() { Code = "RUB", Name = "Рубль" });
                db.Currencies.Add(new Currency() { Code = "USD", Name = "Доллар" });
                db.Currencies.Add(new Currency() { Code = "EUR", Name = "Евро" });
                db.Currencies.Add(new Currency() { Code = "IDR", Name = "Индонезийская рупия" });
                //save
                db.SaveChanges();
            }
        }
    }
}
