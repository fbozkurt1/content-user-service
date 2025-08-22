using ContentService.Application.Requests;
using ContentService.Domain.ApiServices;
using ContentService.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ContentService.Test
{
    public class ContentServiceTests
    {
        private readonly Mock<IContentRepository> _contentRepositoryMock;
        private readonly Mock<ILogger<Application.Services.ContentService>> _loggerMock;
        private readonly Mock<IUserApiService> _userApiServiceMock;
        private readonly Application.Services.ContentService _contentService;

        public ContentServiceTests()
        {
            _contentRepositoryMock = new Mock<IContentRepository>();
            _loggerMock = new Mock<ILogger<Application.Services.ContentService>>();
            _userApiServiceMock = new Mock<IUserApiService>();
            _contentService = new Application.Services.ContentService(_contentRepositoryMock.Object, _loggerMock.Object, _userApiServiceMock.Object);
        }

        [Fact]
        public async Task GetContent_ShouldReturnContentResponse_WhenContentExists()
        {
            // Arrange
            var contentId = 1;
            var content = new Domain.Entities.Content { Id = contentId, Title = "Test", Body = "Test Body" };
            _contentRepositoryMock.Setup(repo => repo.GetById(contentId)).ReturnsAsync(content);

            // Act
            var result = await _contentService.GetContent(contentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contentId, result.Id);
        }

        [Fact]
        public async Task GetContent_ShouldReturnNull_WhenContentDoesNotExist()
        {
            // Arrange
            var contentId = 1;
            _contentRepositoryMock.Setup(repo => repo.GetById(contentId)).ReturnsAsync((Domain.Entities.Content?)null);

            // Act
            var result = await _contentService.GetContent(contentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateContent_ShouldReturnContentResponse_WhenContentIsCreated()
        {
            // Arrange
            var request = new SaveContentRequest { Title = "Test", Body = "Test Body", UserId = 1 };
            var contentId = 1;
            _userApiServiceMock.Setup(api => api.IsUserExist(request.UserId)).ReturnsAsync(true);
            _contentRepositoryMock.Setup(repo => repo.Add(It.IsAny<Domain.Entities.Content>())).ReturnsAsync(contentId);

            // Act
            var result = await _contentService.CreateContent(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contentId, result.Id);
        }

        [Fact]
        public async Task UpdateContent_ShouldReturnUpdatedContentResponse_WhenContentExists()
        {
            // Arrange
            var contentId = 1;
            var request = new SaveContentRequest { Title = "Updated Title", Body = "Updated Body", UserId = 1 };
            var existingContent = new Domain.Entities.Content { Id = contentId, Title = "Old Title", Body = "Old Body", UserId = 1 };
            _userApiServiceMock.Setup(api => api.IsUserExist(request.UserId)).ReturnsAsync(true);
            _contentRepositoryMock.Setup(repo => repo.GetById(contentId)).ReturnsAsync(existingContent);
            _contentRepositoryMock.Setup(repo => repo.Update(It.IsAny<Domain.Entities.Content>())).ReturnsAsync(true);

            // Act
            var result = await _contentService.UpdateContent(contentId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(request.Title, result.Title);
        }

        [Fact]
        public async Task UpdateContent_ShouldReturnNull_WhenContentDoesNotExist()
        {
            // Arrange
            var contentId = 1;
            var request = new SaveContentRequest { Title = "Updated Title", Body = "Updated Body", UserId = 1 };
            _userApiServiceMock.Setup(api => api.IsUserExist(request.UserId)).ReturnsAsync(true);
            _contentRepositoryMock.Setup(repo => repo.GetById(contentId)).ReturnsAsync((Domain.Entities.Content?)null);

            // Act
            var result = await _contentService.UpdateContent(contentId, request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteContent_ShouldReturnTrue_WhenContentIsDeleted()
        {
            // Arrange
            var contentId = 1;
            _contentRepositoryMock.Setup(repo => repo.Delete(contentId)).ReturnsAsync(true);

            // Act
            var result = await _contentService.DeleteContent(contentId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteContent_ShouldReturnFalse_WhenContentIsNotDeleted()
        {
            // Arrange
            var contentId = 1;
            _contentRepositoryMock.Setup(repo => repo.Delete(contentId)).ReturnsAsync(false);

            // Act
            var result = await _contentService.DeleteContent(contentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllContents_ShouldReturnContentListResponse()
        {
            // Arrange
            var page = 1;
            var pageSize = 10;
            var contents = new List<Domain.Entities.Content>
            {
                new Domain.Entities.Content { Id = 1, Title = "Test 1", Body = "Body 1" },
                new Domain.Entities.Content { Id = 2, Title = "Test 2", Body = "Body 2" }
            };
            _contentRepositoryMock.Setup(repo => repo.GetAll(page, pageSize)).ReturnsAsync((contents.Count, contents));

            // Act
            var result = await _contentService.GetAllContents(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contents.Count, result.TotalCount);
        }
    }
}
