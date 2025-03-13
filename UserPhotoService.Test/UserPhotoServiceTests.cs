using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using Moq;
using UserPhotoService.Domain.Entities;
using UserPhotoService.Domain.Ports;

namespace UserPhotoService.Test
{
    public class UserPhotoServiceTests
    {
        private readonly Mock<IUserPhotoRepository> _repositoryMock;
        private readonly Mock<ILogger<Application.Services.UserPhotoService>> _loggerMock;
        private readonly IUserPhotoService _userPhotoService;

        public UserPhotoServiceTests()
        {
            _loggerMock = new Mock<ILogger<Application.Services.UserPhotoService>>();
            _repositoryMock = new Mock<IUserPhotoRepository>();
            _userPhotoService = new Application.Services.UserPhotoService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserPhotoAsync_ExistingUser_ReturnsPhoto()
        {
            var userId = 1;
            var expectedPhoto = new UserPhoto(userId, "base64content", "image/jpg");

            _repositoryMock.Setup(repo => repo.GetUserPhotoAsync(userId))
                .ReturnsAsync(expectedPhoto);

            var result = await _userPhotoService.GetUserPhotoAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal("base64content", result.PhotoBase64);
            Assert.Equal("image/jpg", result.PhotoType);
        }

        [Fact]
        public async Task GetUserPhotoAsync_NonExistingUser_ReturnsNull()
        {
            var userId = 999;
            _repositoryMock.Setup(repo => repo.GetUserPhotoAsync(userId))
                .ReturnsAsync((UserPhoto)null);

            var result = await _userPhotoService.GetUserPhotoAsync(userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllUserPhotosAsync_ReturnsPhotosList()
        {
            var photos = new List<UserPhoto>
            {
                new UserPhoto(1,"base64content1", "image/jpg"),
                new UserPhoto(2,"base64content2", "image/jpg")
            };

            _repositoryMock.Setup(repo => repo.GetAllUserPhotosAsync(1))
                .ReturnsAsync(photos);

            var result = await _userPhotoService.GetAllUserPhotosAsync(1);

            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Equal(1, resultList[0].UserId);
            Assert.Equal("base64content1", resultList[0].PhotoBase64);
            Assert.Equal(2, resultList[1].UserId);
        }

    }

}
