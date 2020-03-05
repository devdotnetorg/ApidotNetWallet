using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace ApidotNetWallet.Services
{
    public class CrossRatesECBService: ICrossRatesService
    {
        public Dictionary<string, decimal> GetCrossRates()
        {
            Dictionary<string, decimal> rates = new Dictionary<string, decimal>();
            var doc = new XmlDocument();
            string code = String.Empty; decimal rate = 0;
            doc.Load(@"http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");
            XmlNodeList nodes = doc.SelectNodes("//*[@currency]");
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
