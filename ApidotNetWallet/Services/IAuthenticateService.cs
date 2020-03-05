using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Services
{
    public interface IAuthenticateService
    {
        public string GetToken(string login);
        //public SymmetricSecurityKey GetSymmetricSecurityKey();
    }
}
