using ApidotNetWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WalletApiContext context;

        private UserRepository users;
        private CurrencyRepository currencies;
        private WalletRepository wallets;

        public UnitOfWork(WalletApiContext context)
        {
            this.context = context;
        }

        public Task Commit() => context.SaveChangesAsync();

        public IUserRepository Users => users ?? (users = new UserRepository(context));
        public ICurrencyRepository Currencies => currencies ?? (currencies = new CurrencyRepository(context));
        public IWalletRepository Wallets => wallets ?? (wallets = new WalletRepository(context));

    }
}
