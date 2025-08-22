using Microsoft.AspNetCore.Mvc;
using Moq;
using UserService.API.Controllers;
using UserService.Application.Requests;
using UserService.Application.Responses;
using UserService.Application.Services;

namespace UserService.Test
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UsersController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetUser_WithExistingId_ShouldReturnOkObjectResult()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new UserResponse(1, "user", "email", DateTime.Now);
            _mockUserService.Setup(s => s.GetUser(userId)).ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserResponse>(okResult.Value);
            Assert.Equal(userId, returnedUser.Id);
        }

        [Fact]
        public async Task GetUser_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var userId = 99;
            _mockUserService.Setup(s => s.GetUser(userId)).ReturnsAsync((UserResponse?)null);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetUsers_WhenUsersExist_ShouldReturnOkResult()
        {
            // Arrange
            var users = new List<UserResponse> { new(1, "user", "email", DateTime.Now) };
            var serviceResult = new UserListResponse(users, 1);
            _mockUserService.Setup(s => s.GetAllUsers(1, 10)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUsers(1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(serviceResult, okResult.Value);
        }

        [Fact]
        public async Task GetUsers_WhenNoUsersExist_ShouldReturnNoContent()
        {
            // Arrange
            var emptyUsers = Enumerable.Empty<UserResponse>();
            var serviceResult = new UserListResponse(emptyUsers, 0);
            _mockUserService.Setup(s => s.GetAllUsers(1, 10)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetUsers(1, 10);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CreateUser_WithValidRequest_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var request = new SaveUserRequest { Username = "newuser", Email = "new@test.com" };
            var createdUser = new UserResponse(101, request.Username, request.Email, DateTime.Now);
            _mockUserService.Setup(s => s.CreateUser(request)).ReturnsAsync(createdUser);

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(UsersController.GetUser), createdAtActionResult.ActionName);
            Assert.Equal(createdUser.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(createdUser, createdAtActionResult.Value);
        }

        [Fact]
        public async Task CreateUser_WithNullRequest_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.CreateUser(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid user data.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateUser_WhenUpdateIsSuccessful_ShouldReturnOk()
        {
            // Arrange
            var userId = 1;
            var request = new SaveUserRequest { Username = "updateduser" };
            var updatedUser = new UserResponse(userId, "updatedUser", "updatedEmail", DateTime.Now);
            _mockUserService.Setup(s => s.UpdateUser(userId, request)).ReturnsAsync(updatedUser);

            // Act
            var result = await _controller.UpdateUser(userId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedUser, okResult.Value);
        }

        [Fact]
        public async Task UpdateUser_WhenUserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var userId = 99;
            var request = new SaveUserRequest { Username = "updateduser" };
            _mockUserService.Setup(s => s.UpdateUser(userId, request)).ReturnsAsync((UserResponse)null);

            // Act
            var result = await _controller.UpdateUser(userId, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUser_WhenDeleteIsSuccessful_ShouldReturnNoContent()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(s => s.DeleteUser(userId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_WhenUserNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var userId = 99;
            _mockUserService.Setup(s => s.DeleteUser(userId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}