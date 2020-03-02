using ApidotNetWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Repositories
{
    public interface IUserRepository : IAsyncRepository<User>
    {
        Task AddWithPassword(User user);
    }
}
