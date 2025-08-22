using ContentService.Application.Requests;
using ContentService.Application.Responses;
using ContentService.Domain.ApiServices;
using ContentService.Domain.Repositories;
using ContentService.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContentService.Application.Services
{
    public class ContentService : IContentService
    {
        private readonly IContentRepository _contentRepository;
        private readonly ILogger<ContentService> _logger;
        private readonly IUserApiService _userApiService;

        public ContentService(IContentRepository contentRepository, ILogger<ContentService> logger, IUserApiService userApiService)
        {
            _contentRepository = contentRepository;
            _logger = logger;
            _userApiService = userApiService;
        }

        public async Task<ContentResponse?> GetContent(int contentId)
        {
            var content = await _contentRepository.GetById(contentId);
            if (content == null)
                return null;

            return new ContentResponse(content);
        }

        public async Task<ContentResponse> CreateContent(SaveContentRequest request)
        {
            //check if user exists before creating content
            await ValidateUserExists(request.UserId);

            var content = new Domain.Entities.Content
            {
                Title = request.Title,
                Body = request.Body,
                UserId = request.UserId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var contentId = await _contentRepository.Add(content);
            content.Id = contentId;
            return new ContentResponse(content);
        }

        public async Task<ContentResponse?> UpdateContent(int contentId, SaveContentRequest request)
        {
            //check if user exists before updating content
            await ValidateUserExists(request.UserId);   

            var existingContent = await _contentRepository.GetById(contentId);
            if (existingContent == null)
                return null;

            existingContent.Title = request.Title;
            existingContent.Body = request.Body;
            existingContent.UpdatedAt = DateTime.Now;
            existingContent.UserId = request.UserId;

            var updated = await _contentRepository.Update(existingContent);
            if (!updated)
                return null;

            return new ContentResponse(existingContent);
        }

        public async Task<bool> DeleteContent(int contentId)
        {
            var isDeleted = await _contentRepository.Delete(contentId);
            if (!isDeleted)
            {
                _logger.LogWarning($"Failed to delete content with Id {contentId}");
                return false;
            }
            return isDeleted;
        }

        public async Task<ContentListResponse> GetAllContents(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var (totalCount, contents) = await _contentRepository.GetAll(page, pageSize);
            var contentResponses = contents.Select(c => new ContentResponse(c)).ToList();
            return new ContentListResponse(totalCount, contentResponses);
        }

        private async Task ValidateUserExists(int userId)
        {
            var userExists = await _userApiService.IsUserExist(userId);
            if (!userExists.HasValue)
            {
                _logger.LogWarning($"User with ID {userId} could not be checked from user api.");
                throw new Exception($"Cannot validate user, please try again later");
            }
            if (userExists.HasValue && !userExists.Value)
            {
                _logger.LogWarning($"User with ID {userId} does not exist.");
                throw new CustomNotificationException($"User with ID {userId} does not exist.");
            }
        }
    }
}
