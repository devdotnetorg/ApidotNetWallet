using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Services
{
    interface ICrossRatesService
    {
        public Dictionary<string, decimal> GetCrossRates();
    }
}
