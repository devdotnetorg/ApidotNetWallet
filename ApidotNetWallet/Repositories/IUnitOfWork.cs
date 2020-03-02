using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Repositories
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ICurrencyRepository Currencies { get; }
        IWalletRepository Wallets { get; }

        Task Commit();
        //IMessageRepository Messages { get; }
    }
}
