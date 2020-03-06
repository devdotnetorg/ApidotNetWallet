using System.Threading.Tasks;
using ApidotNetWallet.Models;
using ApidotNetWallet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApidotNetWallet.Controllers
{
    /// <summary>
    /// Класс для работы со справочником валют
    /// </summary>
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

        /// <summary>
        /// Возвращает массив JSON - справочник валют
        /// </summary>
        /// <returns>
        ///OUT 200 OK JSON
        ///[{"code": "RUB","name": "Рубль"},
        ///{"code": "USD","name": "Доллар"]
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<Currency[]>> Get()
        {
            var currencies = await _db.Currencies.GetSelect(x => new { x.Code, x.Name });
            return Ok(currencies);
        }
    }
}
