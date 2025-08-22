using ContentService.API.Controllers;
using ContentService.Application.Requests;
using ContentService.Application.Responses;
using ContentService.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ContentService.Test
{
    public class ContentsControllerTests
    {
        private readonly Mock<IContentService> _mockContentService;
        private readonly ContentsController _controller;

        public ContentsControllerTests()
        {
            _mockContentService = new Mock<IContentService>();
            _controller = new ContentsController(_mockContentService.Object);
        }


        [Fact]
        public async Task GetContent_WhenContentExists_ShouldReturnOkObjectResult()
        {
            // Arrange
            var contentId = 1;
            var expectedContent = new ContentResponse(1, "test", "bdy", 1, DateTime.Now, null);
            _mockContentService.Setup(s => s.GetContent(contentId)).ReturnsAsync(expectedContent);

            // Act
            var result = await _controller.GetContent(contentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedContent = Assert.IsType<ContentResponse>(okResult.Value);
            Assert.Equal(contentId, returnedContent.Id); 
        }

        [Fact]
        public async Task GetContent_WhenContentDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var contentId = 99;
            _mockContentService.Setup(s => s.GetContent(contentId)).ReturnsAsync((ContentResponse)null);

            // Act
            var result = await _controller.GetContent(contentId);

            // Assert
            Assert.IsType<NotFoundResult>(result); 
        }

        [Fact]
        public async Task GetContents_WhenContentsExist_ShouldReturnOkResult()
        {
            // Arrange
            var contents = new List<ContentResponse> { new ContentResponse(1, "test", "bdy", 1, DateTime.Now, null) };
            var serviceResult = new ContentListResponse(1, contents);
            _mockContentService.Setup(s => s.GetAllContents(1, 10)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetContents(1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(serviceResult, okResult.Value);
        }

        [Fact]
        public async Task GetContents_WhenNoContents_ShouldReturnNoContent()
        {
            // Arrange
            var emptyContents = Enumerable.Empty<ContentResponse>();
            var serviceResult = new ContentListResponse(0, emptyContents);
            _mockContentService.Setup(s => s.GetAllContents(1, 10)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.GetContents(1, 10);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CreateContent_WithValidRequest_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var request = new SaveContentRequest { Title = "New Title", Body = "New Body" };
            var createdContent = new ContentResponse(1, "test", "bdy", 1, DateTime.Now, null);
            _mockContentService.Setup(s => s.CreateContent(request)).ReturnsAsync(createdContent);

            // Act
            var result = await _controller.CreateContent(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(ContentsController.GetContent), createdAtActionResult.ActionName);
            Assert.Equal(createdContent.Id, createdAtActionResult.RouteValues["id"]);
            Assert.Equal(createdContent, createdAtActionResult.Value);
        }

        [Fact]
        public async Task CreateContent_WithNullRequest_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.CreateContent(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid content data.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateContent_WhenUpdateIsSuccessful_ShouldReturnOk()
        {
            // Arrange
            var contentId = 1;
            var request = new SaveContentRequest { Title = "Updated Title" };
            var updatedContent = new ContentResponse(1, "test", "bdy", 1, DateTime.Now, null);
            _mockContentService.Setup(s => s.UpdateContent(contentId, request)).ReturnsAsync(updatedContent);

            // Act
            var result = await _controller.UpdateContent(contentId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedContent, okResult.Value);
        }

        [Fact]
        public async Task UpdateContent_WhenContentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var contentId = 99;
            var request = new SaveContentRequest { Title = "Updated Title" };
            _mockContentService.Setup(s => s.UpdateContent(contentId, request)).ReturnsAsync((ContentResponse)null);

            // Act
            var result = await _controller.UpdateContent(contentId, request);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteContent_WhenDeleteIsSuccessful_ShouldReturnNoContent()
        {
            // Arrange
            var contentId = 1;
            _mockContentService.Setup(s => s.DeleteContent(contentId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteContent(contentId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteContent_WhenContentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var contentId = 99;
            _mockContentService.Setup(s => s.DeleteContent(contentId)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteContent(contentId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteContent_WithInvalidId_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidId = 0;

            // Act
            var result = await _controller.DeleteContent(invalidId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}