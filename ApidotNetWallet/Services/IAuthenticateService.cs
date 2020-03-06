using System;

namespace ApidotNetWallet.Services
{
    /// <summary>
    /// Интерфейс сервиса Аутентификации
    /// </summary>
    public interface IAuthenticateService
    {
        public string GetToken(string login);
    }
}
