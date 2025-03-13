using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBF.Api.Services.Interfaces;
using BBF.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using BBF.Api.DTOs;

namespace BBF.Test.Services
{
    public class UserProfileServiceTests
    {
        private readonly Mock<IUserDataServiceClient> _userDataClientMock;
        private readonly Mock<IUserPhotoServiceClient> _userPhotoClientMock;
        private readonly Mock<ILogger<UserProfileService>> _loggerMock;
        private readonly UserProfileService _service;

        public UserProfileServiceTests()
        {
            _userDataClientMock = new Mock<IUserDataServiceClient>();
            _userPhotoClientMock = new Mock<IUserPhotoServiceClient>();
            _loggerMock = new Mock<ILogger<UserProfileService>>();
            _service = new UserProfileService(_userDataClientMock.Object, _userPhotoClientMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserProfileAsync_WithValidData_ReturnsCombinedProfile()
        {
            int userId = 1;
            string token = "test_token";

            var userData = new UserDataDto
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe"
            };

            var userPhoto = new UserPhotoDto
            {
                UserId = userId,
                PhotoBase64 = "base64data"
            };

            _userDataClientMock.Setup(c => c.GetUserByIdAsync(userId, token))
                .ReturnsAsync(userData);

            _userPhotoClientMock.Setup(c => c.GetUserPhotoAsync(userId, token))
                .ReturnsAsync(userPhoto);

            var result = await _service.GetUserProfileAsync(userId, token);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("base64data", result.PhotoBase64);
        }

        [Fact]
        public async Task GetUserProfileAsync_WithMissingPhoto_ReturnsProfileWithoutPhoto()
        {
            int userId = 1;
            string token = "test_toke";

            var userData = new UserDataDto
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe"
            };

            _userDataClientMock.Setup(c => c.GetUserByIdAsync(userId, token))
                .ReturnsAsync(userData);

            _userPhotoClientMock.Setup(c => c.GetUserPhotoAsync(userId, token))
                .ReturnsAsync((UserPhotoDto)null);

            var result = await _service.GetUserProfileAsync(userId, token);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal(string.Empty, result.PhotoBase64);
        }

        [Fact]
        public async Task GetUserProfileAsync_WithMissingUserData_ReturnsNull()
        {
            int userId = 1;
            string token = "test_token";

            _userDataClientMock.Setup(c => c.GetUserByIdAsync(userId, token))
                .ReturnsAsync((UserDataDto)null);

            var result = await _service.GetUserProfileAsync(userId, token);

            Assert.Null(result);
        }
    }
}
