using ApidotNetWallet.Helper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static ApidotNetWallet.Helper.BaseHelper;

namespace ApidotNetWallet.Services
{
    public class AuthenticationJWTService : IAuthenticateService
    {
        private readonly AppSettings _appSettings;
        public const string ISSUER = "ApidotNetWalletServer"; // издатель токена
        public const string AUDIENCE = "ApidotNetWalletClient"; // потребитель токена
        public const int LIFETIME = 60; // время жизни токена в минутах: = 60 минут
        public AuthenticationJWTService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        //
        private SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));
        }

        public string GetToken(string login)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType,login),
                };
            ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
            //
            var now = DateTime.UtcNow;
            //
            var jwt = new JwtSecurityToken(
                    issuer: AuthenticationJWTService.ISSUER,
                    audience: AuthenticationJWTService.AUDIENCE,
                    notBefore: now,
                    claims: claimsIdentity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthenticationJWTService.LIFETIME)),
                    signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            //
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            //
            return encodedJwt;
        }
    }
}
