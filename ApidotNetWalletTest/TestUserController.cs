using System;
using Xunit;
using ApidotNetWallet.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using ApidotNetWallet.Models;
using ApidotNetWalletTest.Helper;
using Microsoft.AspNetCore.Mvc;

namespace ApidotNetWallet.Test
{
    public class TestUserController
    {
        [Fact]
        public async void PostUser_ShouldReturnUser()
        {
            var user = new User { Email = "test@gmail.com", Password = "test", Name = "test" };
            var mockdb = BaseHelperTest.MockUnitOfWork();
            var mocklogger = new Mock<ILogger<UsersController>>();
            var controller = new UsersController(mocklogger.Object, mockdb.Object);
            var rezuser=await controller.Post(user);
            Assert.Equal(200, (rezuser.Result as OkObjectResult)?.StatusCode);
            var finduser =await mockdb.Object.Users.GetById(user.Id);
            Assert.Equal(user.Email, finduser.Email);
        }
    }
}
