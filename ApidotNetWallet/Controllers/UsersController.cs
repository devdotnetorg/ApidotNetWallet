using System;
using System.Linq;
using System.Threading.Tasks;
using ApidotNetWallet.Helper;
using ApidotNetWallet.Models;
using ApidotNetWallet.Repositories;
using ApidotNetWallet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ApidotNetWallet.Controllers
{
    /// <summary>
    /// Класс для управления пользователями:
    /// создание, изменение настроек, получение токена доступа.
    /// </summary>
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
        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <returns>
        ///OUT 200 OK JSON
        /// {"email":"goga@gmail.com", "name":"goga"}
        /// 400 - Не заполнены все необходимые поля,
        /// Пользователь с Email: уже существует. Укажите другой Email,
        /// Невозможно создать пользователя
        /// </returns>
        /// <param name="user">JSON Аттрибуты нового пользователя.
        /// {"email":"goga@gmail.com", "name":"goga","password":"goga123456"}
        /// </param>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Post([FromBody]User user)
        {
            #region Data Validation
            if (user == null) return NotFound(new { errorText = "Пользователь не найден" });
            user = UserTrim(user);
            // если есть ошибки - возвращаем ошибку 400
            if (!ModelState.IsValid) return BadRequest(ModelState);
            //valid email
            if(!BaseHelper.IsValidEmail(user.Email)) return BadRequest(new { errorText = String.Format("Email: {0} не является адресом электронной почты. Укажите правильный Email вида: user@domain.ltd", user.Email) });
            //unique email verification
            if (await _db.Users.FirstOrDefault(x => x.Email == user.Email) != null)
            {
                return BadRequest(new { errorText = String.Format("Пользователь с Email: {0} уже существует. Укажите другой Email.", user.Email) });
            }
            #endregion
            //
            user.CreateDate = DateTime.Now;
            try
            {
                await _db.Users.AddWithPassword(user);
                //Add Wallet RUB
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
        /// <summary>
        /// Получение аттрибутов пользователя
        /// </summary>
        /// <returns>
        ///OUT 200 OK JSON
        /// {"email":"goga@gmail.com", "name":"goga"}
        /// </returns>
        [HttpGet("info")]
        [Authorize]
        public async Task<ActionResult<string>> GetInfo()
        {
            var emailUser = User.Identity.Name;
            var user = (await _db.Users.GetWhereSelect(u => u.Email == emailUser, u => new { u.Email, u.Name })).FirstOrDefault();
            if (user == null) return NotFound(new { errorText = "Пользователь не найден" });
            return Ok(new { user.Name, user.Email });
        }
        /// <summary>
        /// Изменнеие аттрибутов пользователя
        /// </summary>
        /// <returns>
        ///OUT 200 OK JSON
        /// {"email":"goga@gmail.com", "name":"goga"}
        /// 400 - Новые данные пользователя для изменения отсутствуют,
        /// Не заполнены все необходимые поля,
        /// </returns>
        /// <param name="user">JSON Аттрибуты нового пользователя.
        /// {"name":"goga","password":"goga123456"}
        /// </param>
        [HttpPut("info")]
        [Authorize]
        public async Task<ActionResult<User>> Put([FromBody]User user)
        {
            if (user == null) return NotFound(new { errorText = "Новые данные пользователя для изменения отсутствуют" });
            user = UserTrim(user);
            // если есть ошибки - возвращаем ошибку 400
            if (!ModelState.IsValid) return BadRequest(ModelState);
            //
            var emailUser = User.Identity.Name;
            var newuser = await _db.Users.FirstOrDefault(x => x.Email == emailUser);
            if (newuser == null) return NotFound(new { errorText = "Пользователь не найден" });
            if (user.Name != null) newuser.Name = user.Name;
            if (user.Password != null) newuser.Password = BaseHelper.GetPbkdf2(user.Password);
            //Commit
            await _db.Commit();
            return Ok(new { newuser.Name, newuser.Email });
        }

        /// <summary>
        /// Получение токена
        /// </summary>
        /// <returns>
        ///OUT 200 OK JSON
        /// {token = ТУТ_САМ_ТОКЕН_ДОСТУПА}
        /// 400 - Неверное имя пользователя или пароль,
        /// </returns>
        /// <param name="user">JSON Данные для аутентификации.
        /// {"email":"goga@gmail.com","password":"goga123456"}
        /// </param>
        [HttpPost("token")]
        [AllowAnonymous]
        public ActionResult<string> Token([FromBody]User user)
        {
            user = UserTrim(user);
            //Get Pbkdf2
            var strsha1Password = BaseHelper.GetPbkdf2(user.Password);
            //Find User
            User finduser = _db.Users.FirstOrDefault(x => x.Email == user.Email && x.Password == strsha1Password).Result;
            if (finduser == null) return BadRequest(new { errorText = "Неверное имя пользователя или пароль." });
            var encodedJwt = _authenticationJWTService.GetToken(user.Email);
            //
            return Ok(new { token = encodedJwt });
        }
        /// <summary>
        /// Выполнение удаления пробелов со всех свойств User
        /// </summary>
        private User UserTrim(User user)
        {
            if (user.Email != null) user.Email = user.Email.Trim();
            if (user.Name != null) user.Name = user.Name.Trim();
            if (user.Password != null) user.Password = user.Password.Trim();
            return user;
        }
    }
}
