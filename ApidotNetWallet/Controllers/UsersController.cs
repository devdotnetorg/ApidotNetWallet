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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApidotNetWallet.Controllers
{
    [Route("identification/v1/[controller]")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _db;

        public UsersController(ILogger<UsersController> logger, IUnitOfWork db)
        {
            _logger = logger;
            _db = db;
        }

        //Create User
        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody]User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            // если есть лшибки - возвращаем ошибку 400
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //
            try
            {
                await _db.Users.AddWithPassword(user);
                //Add Wallet
                var currency = await _db.Currencies.FirstOrDefault(x => x.Code == "RUB");
                Wallet RUBWallet = new Wallet() { CurrencyId = currency.Id, UserId = user.Id, Value = 0 };
                //TODO: Add Wallet
                await _db.Wallets.Add(RUBWallet);
            }
            catch (Exception ex)
            {
                return Problem("Cannot create user");
            }
            return Ok(user);
        }

        //Get user
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            var user = await _db.Users.GetById(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        //Change User
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Put([FromBody]User user, int id)
        {
            if (user == null)
            {
                return BadRequest();
            }
            //
            var changeuser = await _db.Users.FirstOrDefault(x => x.Id == id);
            if (changeuser == null)
            {
                return NotFound("User not found");
            }
            //
            user.Name=user.Name.Trim();
            user.Password = user.Password.Trim();
            changeuser.Name = user.Name;
            changeuser.Password = BaseHelper.GetSHA1(user.Password);
            await _db.Commit();
            return Ok(changeuser);
        }

    }
}
