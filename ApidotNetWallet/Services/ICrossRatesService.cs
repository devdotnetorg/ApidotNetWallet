using System.Collections.Generic;

namespace ApidotNetWallet.Services
{
    /// <summary>
    /// Интерфейс получения курса валют
    /// </summary>
    interface ICrossRatesService
    {
        public Dictionary<string, decimal> GetCrossRates();
    }
}
