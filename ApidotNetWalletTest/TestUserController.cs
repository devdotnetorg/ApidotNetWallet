using System;
using Xunit;
using ApidotNetWallet.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using ApidotNetWallet.Models;
using ApidotNetWalletTest.Helper;
using Microsoft.AspNetCore.Mvc;
using ApidotNetWallet.Services;

namespace ApidotNetWallet.Test
{
    public class TestUserController
    {
        [Fact]
        public async void PostUser_ShouldReturnUser()
        {
            var user = new User { Email = "goga@gmail.com", Password = "goga123456", Name = "goga" };
            var mockdb = BaseHelperTest.MockUnitOfWork();
            var mocklogger = new Mock<ILogger<UsersController>>();
            var mockJWTService = BaseHelperTest.AuthenticationJWTService();
 
            //save
            var controller = new UsersController(mocklogger.Object, mockdb.Object, mockJWTService.Object);
            var rezuser=await controller.Post(user);
            //
            Assert.Equal(200, (rezuser.Result as OkObjectResult)?.StatusCode);
        }
    }
}
