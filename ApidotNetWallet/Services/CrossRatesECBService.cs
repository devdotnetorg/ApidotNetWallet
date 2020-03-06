using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace ApidotNetWallet.Services
{
    /// <summary>
    /// Сервис получения курса валют
    /// </summary>
    public class CrossRatesECBService: ICrossRatesService
    {
        
        /// <summary>
        /// Получить курс валют
        /// </summary>
        /// <returns>
        /// Курсы валют в формате: код валюты, значение множителя
        /// </returns>
        public Dictionary<string, decimal> GetCrossRates()
        {
            Dictionary<string, decimal> rates = new Dictionary<string, decimal>();
            var doc = new XmlDocument();
            doc.Load(@"http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");
            XmlNodeList nodes = doc.SelectNodes("//*[@currency]");
            string code = String.Empty; decimal rate = 0;
            if (nodes != null)
            {
                foreach (XmlNode node in nodes)
                {
                    code = node.Attributes["currency"].Value;
                    rate = Decimal.Parse(node.Attributes["rate"].Value, NumberStyles.Any, new CultureInfo("en-Us"));
                    //
                    rates.Add(code, rate);
                }
            }
            //
            return rates;
        }
    }
}
