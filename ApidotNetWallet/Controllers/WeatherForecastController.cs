using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApidotNetWallet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApidotNetWallet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WalletApiContext _db;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WalletApiContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("adduser")]
        public WeatherForecast adduser()
        {
            var today = new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = 23,
                Summary = "spb"
            };
            //
            var sha1 = new SHA1CryptoServiceProvider();
            var strPassword = "123456";
            var bytePassword = Encoding.ASCII.GetBytes(strPassword);
            var strsha1Password = Encoding.Default.GetString(sha1.ComputeHash(bytePassword));
            // создаем два объекта User
            User user1 = new User { Name = "Tom", Email= "Tom@gmail.com",Password= strsha1Password };
            User user2 = new User { Name = "Alice", Email = "Alice@gmail.com", Password = strsha1Password };

            _db.Users.Add(user1);
            _db.Users.Add(user2);

            _db.SaveChanges();

            //
            return today; 
        }

        [HttpGet("getpass")]
        public WeatherForecast getpass()
        {
            var today = new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = 23,
                Summary = "spb"
            };
            //
            var sha1 = new SHA1CryptoServiceProvider();
            var strPassword = "123456";
            var bytePassword = Encoding.ASCII.GetBytes(strPassword);
            var strsha1Password = Encoding.Default.GetString(sha1.ComputeHash(bytePassword));
            //
            var user = _db.Users
               .Where(b => b.Email == "Tom@gmail.com")
               .FirstOrDefault();
            //
            if(strsha1Password==user.Password)
            {
                Debug.Write("ok");
            }

            //
            return today;
        }

        [HttpGet("createtoken")]
        public WeatherForecast createtoken()
        {
            var today = new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = 23,
                Summary = "spb"
            };
            //
            Token token = new Token();
            token.Value = Guid.NewGuid();
            var user = _db.Users
                .Where(b => b.Email == "Tom@gmail.com")
                .FirstOrDefault();
            token.User = user;
            token.ExpiredDate = (DateTime.Now).AddHours(24);
            _db.Tokens.Add(token);

            _db.SaveChanges();

            //
            return today;
        }
    }
}
