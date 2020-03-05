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
using Microsoft.AspNetCore.Authorization;
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
        //GET Get All wallet
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<Wallet[]>> GetAll()
        {
            var emailUser = User.Identity.Name;
            var user = await _db.Users.FirstOrDefault(x => x.Email == emailUser);
            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }
            //
            var wallets = await _db.Wallets.GetWhereSelect(x => x.UserId == user.Id,(x) => new { x.Currency.Code, x.Currency.Name,x.Value});
            return Ok(wallets);
        }
        //GET Get wallet id
        [Authorize]
        [HttpGet("{code}")]
        public async Task<ActionResult<Wallet>> GetWallet(string code)
        {
            var emailUser = User.Identity.Name;
            var user = await _db.Users.FirstOrDefault(x => x.Email == emailUser);
            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }
            //
            var wallet = (await _db.Wallets.GetWhereSelect(x => x.UserId == user.Id && x.Currency.Code== code, (x) => new { x.Currency.Code, x.Currency.Name, x.Value })).FirstOrDefault();
            if(wallet==null) return NotFound(new { errorText = "Кошелек с таким кодом валюты не найден" });
            return Ok(wallet);
        }
        //PUT Create wallet
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<Wallet>> CreateWallet([FromBody]Currency currency)
        {
            var emailUser = User.Identity.Name;
            var user = await _db.Users.FirstOrDefault(x => x.Email == emailUser);
            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }
            //
            if (currency == null)
            {
                return NotFound(new { errorText = "Не указан код валюты" });
            }
            //
            if (currency.Code == null)
            {
                return NotFound(new { errorText = "Не указан код валюты" });
            }
            //
            var findcurrency = await _db.Currencies.FirstOrDefault(x => x.Code == currency.Code);
            if (findcurrency == null)
            {
                return NotFound(new { errorText = "Указанный код валюты кошелька не найден" });
            }
            //
            if (user.Wallets.FirstOrDefault(x=>x.Currency.Id== findcurrency.Id)!=null) return BadRequest(new { errorText = "Кошелек с указанным кодом валюты уже существует" });
            //
            Wallet newWallet = new Wallet() { Id=Guid.NewGuid(), User = user, Currency = findcurrency, Value = 0 };
            await _db.Wallets.Add(newWallet);
            //
            return Ok(new { newWallet.Currency.Code, newWallet.Currency.Name, newWallet.Value});
        }
        //DELETE Delete wallet
        [Authorize]
        [HttpDelete("{code}")]
        public async Task<ActionResult<Wallet>> DeleteWallet(string code)
        {
            var emailUser = User.Identity.Name;
            var user = await _db.Users.FirstOrDefault(x => x.Email == emailUser);
            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }
            //
            var findcurrency = await _db.Currencies.FirstOrDefault(x => x.Code == code);
            if (findcurrency == null)
            {
                return NotFound(new { errorText = "Указанный код валюты кошелька не найден" });
            }
            //
            var findwallet = user.Wallets.FirstOrDefault(x => x.Currency.Id == findcurrency.Id);
            if (findwallet == null)
            {
                return NotFound(new { errorText = "Кошелек с указанным кодом валюты не найден" });
            }
            //
            if(findwallet.Currency.Code=="RUB") return BadRequest(new { errorText = "Невозможно удалить кошелек по умолчанию RUB" });
            //
            if (findwallet.Value>0) return BadRequest(new { errorText = "Невозможно удалить кошелек с не нулевым балансом" });
            //
            await _db.Wallets.Remove(findwallet);
            //
            return Ok(new { Text = "Кошелек успешно удален" });
        }
    }
}
