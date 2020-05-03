using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
            //папка с приложением
            var contentRoot = Environment.CurrentDirectory;
            //заполнение currencies
            //проверка наличие файла справочника currencies
            var strPathFile = String.Empty;
            strPathFile = contentRoot + "/seeddata/currencies.json";
            if (System.IO.File.Exists(strPathFile))
                {
                //string json = File.ReadAllText(strPathFile).ToString();
                string json = new StreamReader(strPathFile, System.Text.Encoding.UTF8).ReadToEnd();
                var output = JsonConvert.DeserializeObject<List<Currency>>(json);
                var dictionary = output.ToDictionary(x => x.Code, y => y.Name);

                    //Заполнение Таблицы
                    foreach(var item in dictionary)
                        {
                            if(db.Currencies.FirstOrDefault(x=>x.Code==item.Key)==null)
                                {
                                    //добавляем запись
                                    db.Currencies.Add(new Currency() { Code = item.Key, Name = item.Value });
                                }
                        }
                    //save
                    db.SaveChanges();
                }else
                {
                    throw new Exception("Отсутствует справочник currencies в папке /seeddata/");
                }

            /*
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
            */
        }
    }
}
