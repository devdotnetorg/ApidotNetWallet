using ApidotNetWallet.Helper;
using ApidotNetWallet.Models;
using ApidotNetWallet.Repositories;
using ApidotNetWallet.Services;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApidotNetWalletTest.Helper
{
    public static class BaseHelperTest
    {
        public static Mock<IUnitOfWork> MockUnitOfWork()
        {
            var users = new Dictionary<Guid, User>();
            var currencies = new List<Currency>();
            var mockUserRepository = new Mock<IUserRepository>();
            var mockCurrencyRepository = new Mock<ICurrencyRepository>();
            var mockWalletRepository = new Mock<IWalletRepository>();

            //Заполнение справочника
            currencies.Add(new Currency() { Id = Guid.NewGuid(), Code = "RUB", Name = "Рубль" });
            currencies.Add(new Currency() { Id = Guid.NewGuid(), Code = "RUB", Name = "Рубль" });
            currencies.Add(new Currency() { Id = Guid.NewGuid(), Code = "USD", Name = "Доллар" });
            currencies.Add(new Currency() { Id = Guid.NewGuid(), Code = "EUR", Name = "Евро" });
            currencies.Add(new Currency() { Id = Guid.NewGuid(), Code = "IDR", Name = "Индонезийская рупия" });       

            //Add Подмена методов которые будем использовать
            //mockUserRepository.Setup(c => c.Add(It.IsAny<User>()))
            //    .Returns<User>((c) => Task.Run(() => { users.Add(c); c.Id = users.IndexOf(c); }));
            //AddWithPassword Подмена методов которые будем использовать

            mockUserRepository.Setup(c => c.AddWithPassword(It.IsAny<User>()))
                .Returns<User>((c) => Task.Run(() =>
                {
                    c.Password = BaseHelper.GetPbkdf2(c.Password);
                    Guid _guid = Guid.NewGuid();
                    users.Add(_guid, c); c.Id = _guid;
                }));

            mockUserRepository.Setup(c => c.GetById(It.IsAny<Guid>()))
                .Returns<Guid>((id) => Task.FromResult(users[id]));

            mockCurrencyRepository.Setup(c => c.FirstOrDefault(It.IsAny<Expression<Func<Currency, bool>>>()))
                .Returns < Expression<Func<Currency, bool>>>(x=> Task.FromResult(currencies.Where(x.Compile()).FirstOrDefault()));


            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.SetupGet(d => d.Users).Returns(mockUserRepository.Object);
            mockUnitOfWork.SetupGet(d => d.Currencies).Returns(mockCurrencyRepository.Object);
            mockUnitOfWork.SetupGet(d => d.Wallets).Returns(mockWalletRepository.Object);

            return mockUnitOfWork;
        }
        public static Mock<IAuthenticateService> AuthenticationJWTService()
        {
            var mock = new Mock<IAuthenticateService>();
            var key = "djd7rBZkOw35Es2vF6jxoReVchT0E6XnuAhcZ0Mn04k =";
            mock.Setup(x => x.GetToken(It.IsAny<string>())).Returns<string>(x => key);
            return mock;
        }
    }
}
