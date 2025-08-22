using Microsoft.Extensions.Logging;
using Moq;
using UserService.Application.Requests;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Exceptions;

namespace UserService.Test
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<Application.Services.UserService>> _loggerMock;
        private readonly UserService.Application.Services.UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<Application.Services.UserService>>();
            _userService = new UserService.Application.Services.UserService(_userRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUser_ShouldReturnUserResponse_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Username = "testuser", Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUser_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            _userRepositoryMock.Setup(repo => repo.GetById(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetUser(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnUserResponse_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var request = new SaveUserRequest { Username = "newuser", Email = "newuser@example.com" };
            _userRepositoryMock.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(1);

            // Act
            var result = await _userService.CreateUser(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Username, result.Username);
            Assert.Equal(request.Email, result.Email);
        }

        [Fact]
        public async Task CreateUser_ShouldThrowException_WhenUserCreationFails()
        {
            // Arrange
            var request = new SaveUserRequest { Username = "newuser", Email = "newuser@example.com" };
            _userRepositoryMock.Setup(repo => repo.Add(It.IsAny<User>())).ReturnsAsync(0);

            // Act & Assert
            await Assert.ThrowsAsync<CustomNotificationException>(() => _userService.CreateUser(request));
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnUpdatedUserResponse_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var existingUser = new User { Id = userId, Username = "olduser", Email = "old@example.com" };
            var request = new SaveUserRequest { Username = "updateduser", Email = "updated@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetById(userId)).ReturnsAsync(existingUser);
            _userRepositoryMock.Setup(repo => repo.Update(It.IsAny<User>())).ReturnsAsync(true);

            // Act
            var result = await _userService.UpdateUser(userId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Username, result.Username);
            Assert.Equal(request.Email, result.Email);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            var request = new SaveUserRequest { Username = "updateduser", Email = "updated@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetById(userId)).ReturnsAsync((User?)null);

            // Act
            var result = await _userService.UpdateUser(userId, request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnTrue_WhenUserIsDeletedSuccessfully()
        {
            // Arrange
            var userId = 1;
            _userRepositoryMock.Setup(repo => repo.Delete(userId)).ReturnsAsync(true);

            // Act
            var result = await _userService.DeleteUser(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnFalse_WhenUserDeletionFails()
        {
            // Arrange
            var userId = 1;
            _userRepositoryMock.Setup(repo => repo.Delete(userId)).ReturnsAsync(false);

            // Act
            var result = await _userService.DeleteUser(userId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnPagedUserListResponse()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Email = "user1@example.com" },
                new User { Id = 2, Username = "user2", Email = "user2@example.com" }
            };
            _userRepositoryMock.Setup(repo => repo.GetAll(page, pageSize)).ReturnsAsync((users.Count, users));

            // Act
            var result = await _userService.GetAllUsers(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(users.Count, result.Users.Count());
            Assert.Equal(users.Count, result.TotalCount);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            _userRepositoryMock.Setup(repo => repo.GetAll(page, pageSize)).ReturnsAsync((0, new List<User>()));

            // Act
            var result = await _userService.GetAllUsers(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Users);
            Assert.Equal(0, result.TotalCount);
        }
    }
}
