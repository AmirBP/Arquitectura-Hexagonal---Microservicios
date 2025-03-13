using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using UserDataService.Domain.Entities;
using UserDataService.Domain.Ports;

namespace UserDataService.Test
{
    public class UserDataServiceTests
    {
        private readonly Mock<IUserDataRepository> _repositoryMock;
        private readonly Mock<ILogger<Application.Services.UserDataService>> _loggerMock;
        private readonly IUserDataService _userDataService;

        public UserDataServiceTests()
        {
            _repositoryMock = new Mock<IUserDataRepository>();
            _loggerMock = new Mock<ILogger<Application.Services.UserDataService>>();
            _userDataService = new Application.Services.UserDataService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_Existing_ReturnsUser()
        {
            var userId = 1;
            var expectedUser = new UserData(userId, "John", "Doe", "base64image");

            _repositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            var result = await _userDataService.GetUserByIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("base64image", result.Avatar);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingUser_ReturnsNull()
        {
            var userid = 999;

            _repositoryMock.Setup(repo => repo.GetUserByIdAsync(userid))
                .ReturnsAsync((UserData?)null);

            var result = await _userDataService.GetUserByIdAsync(userid);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUserList()
        {
            var users = new List<UserData>
            {
                new UserData(1, "John", "Doe", "base64image"),
                new UserData(2, "Jane", "Doe", "base64image")
            };

            _repositoryMock.Setup(repo => repo.GetAllUsersAsync(1))
                .ReturnsAsync(users);

            var result = await _userDataService.GetAllUsersAsync(1);

            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal(1, resultList[0].Id);
            Assert.Equal("John", resultList[0].FirstName);
            Assert.Equal(2, resultList[1].Id);
            Assert.Equal("Jane", resultList[1].FirstName);
        }
    }
}
