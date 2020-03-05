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
using ApidotNetWallet.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IAuthenticateService _authenticationJWTService;

        public UsersController(ILogger<UsersController> logger, IUnitOfWork db, IAuthenticateService authenticationJWTService)
        {
            _logger = logger;
            _db = db;
            _authenticationJWTService = authenticationJWTService;
        }
        private User UserTrim (User user)
        {
            if (user.Email != null) user.Email = user.Email.Trim();
            if (user.Name != null) user.Name = user.Name.Trim();
            if (user.Password != null) user.Password = user.Password.Trim();
            //
            return user;
        }
        //POST Create User
        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody]User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            // если есть ошибки - возвращаем ошибку 400
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //
            user = UserTrim(user);
            //unique email verification
            if (await _db.Users.FirstOrDefault(x=>x.Email==user.Email)!=null)
            {
                return BadRequest(new { errorText = String.Format("Пользователь с Email: {0} уже существует. Укажите другой Email.", user.Email)});
            }
            //
            user.CreateDate = DateTime.Now;
            try
            {
                await _db.Users.AddWithPassword(user);
                //Add Wallet
                var currency = await _db.Currencies.FirstOrDefault(x => x.Code == "RUB");
                Wallet RUBWallet = new Wallet() { CurrencyId = currency.Id, UserId = user.Id, Value = 0 };
                await _db.Wallets.Add(RUBWallet);
            }
            catch (Exception ex)
            {
                return BadRequest(new { errorText = "Невозможно создать пользователя" });
            }
            return Ok(new { user.Name, user.Email });
        }
        //GET Get User
        [Authorize]
        [HttpGet("info")]
        public async Task<ActionResult<string>> GetInfo()
        {
            var emailUser = User.Identity.Name;
            var user = (await _db.Users.GetWhereSelect(u => u.Email == emailUser, u => new { u.Email, u.Name })).FirstOrDefault();
            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден"});
            }
            return Ok(new { user.Name, user.Email });
        }
        //PUT Change User
        [Authorize]
        [HttpPut("info")]
        public async Task<ActionResult<User>> Put([FromBody]User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            // если есть ошибки - возвращаем ошибку 400
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            //
            user = UserTrim(user);
            //
            var emailUser = User.Identity.Name;
            //
            var newuser = await _db.Users.FirstOrDefault(x => x.Email == emailUser);
            if (newuser == null) return NotFound(new { errorText = "Пользователь не найден" });
            //
            if(user.Name!=null) newuser.Name= user.Name;
            if(user.Password!=null) newuser.Password = BaseHelper.GetPbkdf2(user.Password);
            //Commit
            await _db.Commit();
            return Ok(new { newuser.Name, newuser.Email });
        }

        //GET Get token
        [HttpPost("token")]
        public async Task<ActionResult<string>> Token([FromBody]User user)
        {
            //
            user = UserTrim(user);
            //Get Pbkdf2
            var strsha1Password = BaseHelper.GetPbkdf2(user.Password);
            //Find User
            User finduser = _db.Users.FirstOrDefault(x => x.Email == user.Email && x.Password == strsha1Password).Result;
            if (finduser == null) BadRequest(new { errorText = "Invalid username or password." });
            //
            var encodedJwt=_authenticationJWTService.GetToken(user.Email);
            //
            return Ok(new {token = encodedJwt});
        }
    }
        
}
