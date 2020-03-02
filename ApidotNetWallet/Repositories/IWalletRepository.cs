using ApidotNetWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Repositories
{
    public interface IWalletRepository : IAsyncRepository<Wallet>
    {
        
    }
}
