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
    public class CurrenciesController : Controller
    {
        private readonly ILogger<CurrenciesController> _logger;
        private readonly IUnitOfWork _db;

        public CurrenciesController(ILogger<CurrenciesController> logger, IUnitOfWork db)
        {
            _logger = logger;
            _db = db;
        }

        //Get Currencies
        [HttpGet]
        public async Task<ActionResult<Currency[]>> Get()
        {
            var currencies = (await _db.Currencies.GetAll()).ToArray();
            return Ok(currencies);
        }
    }
}
