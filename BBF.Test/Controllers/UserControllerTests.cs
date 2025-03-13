using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBF.Api.Controllers;
using BBF.Api.DTOs;
using BBF.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BBF.Test.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserProfileService> _userProfileServiceMock;
        private readonly Mock<ILogger<UserController>> _loggerMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userProfileServiceMock = new Mock<IUserProfileService>();
            _loggerMock = new Mock<ILogger<UserController>>();
            _controller = new UserController(_userProfileServiceMock.Object, _loggerMock.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer test_token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetUserProfile_ExistingUser_ReturnsOk()
        {
            int userId = 1;
            var expectedProfile = new UserProfileDto
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                PhotoBase64 = "base64data"
            };

            _userProfileServiceMock.Setup(s => s.GetUserProfileAsync(userId, It.IsAny<string>()))
                .ReturnsAsync(expectedProfile);

            // Act
            var result = await _controller.GetUserProfile(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserProfileDto>(okResult.Value);
            Assert.Equal(userId, returnValue.Id);
            Assert.Equal("John", returnValue.FirstName);
            Assert.Equal("Doe", returnValue.LastName);
            Assert.Equal("base64data", returnValue.PhotoBase64);
        }

        [Fact]
        public async Task GetUserProfile_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            int userId = 999;
            _userProfileServiceMock.Setup(s => s.GetUserProfileAsync(userId, It.IsAny<string>()))
                .ReturnsAsync((UserProfileDto)null);

            // Act
            var result = await _controller.GetUserProfile(userId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetAllUserProfiles_ReturnsOkWithList()
        {
            // Arrange
            var expectedList = new UserListDto
            {
                Page = 1,
                TotalPages = 2,
                Users = new List<UserProfileDto>
                {
                    new UserProfileDto { Id = 1, FirstName = "John", LastName = "Doe", PhotoBase64 = "base64-1" },
                    new UserProfileDto { Id = 2, FirstName = "Jane", LastName = "Smith", PhotoBase64 = "base64-2" }
                }
            };

            _userProfileServiceMock.Setup(s => s.GetAllUserProfilesAsync(1, It.IsAny<string>()))
                .ReturnsAsync(expectedList);

            // Act
            var result = await _controller.GetAllUserProfiles(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserListDto>(okResult.Value);
            Assert.Equal(2, returnValue.Users.Count);
            Assert.Equal(1, returnValue.Page);
            Assert.Equal(2, returnValue.TotalPages);
        }
    }
}
