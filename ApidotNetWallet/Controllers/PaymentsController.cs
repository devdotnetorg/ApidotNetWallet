using ApidotNetWallet.Helper;
using ApidotNetWallet.Models;
using ApidotNetWallet.Repositories;
using ApidotNetWallet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;

namespace ApidotNetWallet.Controllers
{
    [Route("sinap/v1/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _db;
        private readonly IAuthenticateService _authenticationJWTService;
        //
        private static readonly object Locker = new object();
        private static Dictionary<string, decimal> _rates = new Dictionary<string, decimal>();
        private static DateTime _dateLoadCrossRates;

        public PaymentsController(ILogger<UsersController> logger, IUnitOfWork db, IAuthenticateService authenticationJWTService)
        {
            _logger = logger;
            _db = db;
            _authenticationJWTService = authenticationJWTService;
        }
        //GET Get All CrossRates
        [HttpGet("crossRates")]
        public async Task<ActionResult<string>> GetAll()
        {
            //
            UpdateCrossRates();
            //
            var outRates = _rates.Select((x) => new { Code = x.Key, Rate = x.Value });
            //
            return Ok(outRates);
        }
        //GET Get id CrossRates
        [HttpGet("crossRates/{id}")]
        public async Task<ActionResult<string>> GetAll(string id)
        {
            //
            UpdateCrossRates();
            //
            var outRate = _rates.FirstOrDefault(x => x.Key == id);
            if (outRate.Key == null) return NotFound(new { errorText = "Код валюты не наден" });
            //
            return Ok(new { Code = outRate.Key, Rate = outRate.Value });
        }
        //POST Пополнение с карты
        [Authorize]
        [HttpPost("1")]
        public async Task<ActionResult<string>> PaymentCardToWallet([FromBody]JObject raw)
        {
            /*
             {
                "numbercard":"123456",
                "codewallet": "RUB",
                "amount": 10.1
              }
             */
            var emailUser = User.Identity.Name;
            var user = await _db.Users.FirstOrDefault(x => x.Email == emailUser);
            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }
            //
            var numbercard = (string)raw["numbercard"];
            if (numbercard == null) return BadRequest(new { errorText = "Не указан номер карты" });
            var codewallet = (string)raw["codewallet"];
            if (codewallet == null) return BadRequest(new { errorText = "Не указан кошелек для зачисления" });
            var amount = (decimal)raw["amount"];
            if (amount <= 0) return BadRequest(new { errorText = "Не верно указана сумма для зачисления. Или сумма должна быть больше нуля" });
            //
            var currencyBuy = "RUB";
            var currencySell = await _db.Currencies.FirstOrDefault(x => x.Code == codewallet);
            if (currencySell == null)
            {
                return NotFound(new { errorText = "Указанный код валюты кошелька не найден" });
            }
            //
            var wallet = user.Wallets.FirstOrDefault(x => x.Currency.Id == currencySell.Id);
            if (wallet == null) return NotFound(new { errorText = "Кошелек с указанным кодом валюты не существует" });
            //Transfer
            UpdateCrossRates();
            var rateBuy = _rates.FirstOrDefault(x => x.Key == currencyBuy);
            var rateSell = _rates.FirstOrDefault(x => x.Key == currencySell.Code);
            decimal sum = 0;
            if (rateBuy.Key != rateSell.Key)
            {
                sum = BaseHelper.BuySell(rateBuy.Value, amount, rateSell.Value);
            }
            else
            {
                sum = amount;
            }
            wallet.Value += sum;
            //
            await _db.Commit();
            //
            return Ok(new { wallet.Currency.Code, wallet.Currency.Name, wallet.Value });
        }

        //POST Снятие с кошелька
        [Authorize]
        [HttpPost("2")]
        public async Task<ActionResult<string>> PaymentWalletToCard([FromBody]JObject raw)
        {
            /*
             {
                "numbercard":"123456",
                "codewallet": "RUB",
                "amount": 10.1
              }
             */
            var emailUser = User.Identity.Name;
            var user = await _db.Users.FirstOrDefault(x => x.Email == emailUser);
            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }
            //
            var numbercard = (string)raw["numbercard"];
            if (numbercard == null) return BadRequest(new { errorText = "Не указан номер карты" });
            var codewallet = (string)raw["codewallet"];
            if (codewallet == null) return BadRequest(new { errorText = "Не указан кошелек для зачисления" });
            var amount = (decimal)raw["amount"];
            if (amount <= 0) return BadRequest(new { errorText = "Не верно указана сумма для зачисления. Или сумма должна быть больше нуля" });
            //
            var currencyBuy = await _db.Currencies.FirstOrDefault(x => x.Code == codewallet);
            var currencySell = "RUB";
            if (currencyBuy == null)
            {
                return NotFound(new { errorText = "Указанный код валюты кошелька не найден" });
            }
            //
            var wallet = user.Wallets.FirstOrDefault(x => x.Currency.Code == currencySell);
            if (wallet == null) return NotFound(new { errorText = "Кошелек с указанным кодом валюты не существует" });
            //Проверка минимальных средств
            if (wallet.Value < amount) return BadRequest(new { errorText = "У Вас недостаточно средств для снятия с кошелька" });
            //Transfer
            decimal sum = 0;
            sum = amount;
            wallet.Value -= sum;
            //
            await _db.Commit();
            //
            return Ok(new { wallet.Currency.Code, wallet.Currency.Name, wallet.Value });
        }

        //POST Перевод между счетами
        [Authorize]
        [HttpPost("3")]
        public async Task<ActionResult<string>> PaymentWalletToWallet([FromBody]JObject raw)
        {
            /* IN
             {
                "codewallet1":"RUB",
                "codewallet2": "USD",
                "amount": 10.1
              }
              OUT

             */
            var emailUser = User.Identity.Name;
            var user = await _db.Users.FirstOrDefault(x => x.Email == emailUser);
            if (user == null)
            {
                return NotFound(new { errorText = "Пользователь не найден" });
            }
            //
            var codewallet1 = (string)raw["codewallet1"];
            if (codewallet1 == null) return BadRequest(new { errorText = "Не указан кошелек с которого будут переводиться средства" });
            var codewallet2 = (string)raw["codewallet2"];
            if (codewallet2 == null) return BadRequest(new { errorText = "Не указан кошелек на который будет перевод средств" });
            if (codewallet1 == codewallet2) return BadRequest(new { errorText = "Нельзя перевести средства на тот же кошелек. Необходимо для получения средств выбрать другой кошелек" });
            var amount = (decimal)raw["amount"];
            if (amount <= 0) return BadRequest(new { errorText = "Не верно указана сумма для зачисления. Или сумма должна быть больше нуля" });
            //
            var currencyBuy = await _db.Currencies.FirstOrDefault(x => x.Code == codewallet1);
            var currencySell = await _db.Currencies.FirstOrDefault(x => x.Code == codewallet2);
            if (currencyBuy == null|| currencySell == null)
            {
                return NotFound(new { errorText = "Указанный код валюты кошелька не найден" });
            }
            //
            var wallet1 = user.Wallets.FirstOrDefault(x => x.Currency.Id == currencyBuy.Id);
            var wallet2 = user.Wallets.FirstOrDefault(x => x.Currency.Id == currencySell.Id);
            //
            if (wallet1 == null || wallet2 == null) return NotFound(new { errorText = "Кошелек с указанным кодом валюты не существует" });
            //
            //Проверка минимальных средств
            if (wallet1.Value < amount) return BadRequest(new { errorText = "У Вас недостаточно средств для перевода с кошелька" });
            //Transfer
            UpdateCrossRates();
            var rateBuy = _rates.FirstOrDefault(x => x.Key == currencyBuy.Code);
            var rateSell = _rates.FirstOrDefault(x => x.Key == currencySell.Code);
            decimal sum = 0;
            sum = BaseHelper.BuySell(rateBuy.Value, amount, rateSell.Value);
            //
            wallet1.Value -= amount;
            wallet2.Value += sum;
            //
            await _db.Commit();
            //
            var responseArray = new[] { new { wallet1.Currency.Code, wallet1.Currency.Name, wallet1.Value }, new { wallet2.Currency.Code, wallet2.Currency.Name, wallet2.Value } };
            //
            return Ok(responseArray);
        }


            private void UpdateCrossRates()
        {
            //Update
            lock (Locker)
            {
                //Частота обновление курса
                var updateFrequency = new TimeSpan(0, 5, 0);//раз в 5 минут
                TimeSpan diff1 = DateTime.Now - _dateLoadCrossRates;
                if (diff1 > updateFrequency)
                {
                    ICrossRatesService crossRatesService = new CrossRatesECBService();
                    _rates = crossRatesService.GetCrossRates();
                    //add EUR
                    _rates.Add("EUR", 1);
                    //
                    _dateLoadCrossRates = DateTime.Now;
                }
            }






        }
    }
}