using ApidotNetWallet.Helper;
using ApidotNetWallet.Models;
using ApidotNetWallet.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApidotNetWalletTest.Helper
{
    public static class BaseHelperTest
    {
        public static Mock<IUnitOfWork> MockUnitOfWork()
        {
            var users = new List<User>();
            var mockUserRepository = new Mock<IUserRepository>();

            //Add Подмена методов которые будем использовать
            //mockUserRepository.Setup(c => c.Add(It.IsAny<User>()))
            //    .Returns<User>((c) => Task.Run(() => { users.Add(c); c.Id = users.IndexOf(c); }));
            //AddWithPassword Подмена методов которые будем использовать
            mockUserRepository.Setup(c => c.AddWithPassword(It.IsAny<User>()))
                .Returns<User>((c) => Task.Run(() => { c.Password = BaseHelper.GetSHA1(c.Password); users.Add(c); c.Id = users.IndexOf(c); }));

            mockUserRepository.Setup(c => c.GetById(It.IsAny<int>()))
                .Returns<int>((id) => Task.FromResult(users[id]));

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.SetupGet(d => d.Users).Returns(mockUserRepository.Object);

            return mockUnitOfWork;
        }
    }
}
