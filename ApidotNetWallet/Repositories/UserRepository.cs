using ApidotNetWallet.Helper;
using ApidotNetWallet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApidotNetWallet.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(WalletApiContext context) : base(context) { }

        public Task AddWithPassword(User user)
        {
            user.Password=BaseHelper.GetSHA1(user.Password);
            return Add(user);
        }
    }
}
