using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApidotNetWallet.Helper;
using ApidotNetWallet.Models;
using ApidotNetWallet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApidotNetWallet.Controllers
{
    [Route("funding-sources/v1/[controller]")]
    public class WalletsController : Controller
    {
        private readonly ILogger<WalletsController> _logger;
        private readonly IUnitOfWork _db;

        public WalletsController(ILogger<WalletsController> logger, IUnitOfWork db)
        {
            _logger = logger;
            _db = db;
        }

        //Create wallet

        //Delete wallet

        //All
        [HttpGet("user/{id}")]
        public async Task<ActionResult<Wallet[]>> Get(int id)
        {
            var currencies = await _db.Wallets.GetWhere(x => x.UserId == id);
                 


            //var currencies = await _db.Wallets.GetWhere(x => x.UserId == id);

            /*
            
            .Users.Include(u => u.Roles).Where(u => u.Username == username);
            User _user = IQUsers.FirstOrDefault<User>();
            Role _role = _user.Roles.FirstOrDefault<Role>();
            roleName = _role.RoleName;


            var changeuser = await _db.Wallets.GetAll(x => x.UserId == idUser);

            var currencies = (await _db.Currencies..wh.GetAll()).ToArray();
            */
            return Ok(currencies);
        }



    }
}
