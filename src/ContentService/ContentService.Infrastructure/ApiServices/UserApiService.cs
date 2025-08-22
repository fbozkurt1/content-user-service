using ContentService.Domain.ApiServices;
using ContentService.Domain.ApiServices.Dtos;
using ContentService.Infrastructure.ApiServices.Refits;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContentService.Infrastructure.ApiServices
{
    public class UserApiService : IUserApiService
    {
        private readonly IUserApiRefitClient _userApiRefitClient;
        private readonly ILogger<UserApiService> _logger;

        public UserApiService(IUserApiRefitClient userApiRefitClient, ILogger<UserApiService> logger)
        {
            _userApiRefitClient = userApiRefitClient;
            _logger = logger;
        }

        public async Task<int?> CreateUser(CreateUserDto request)
        {
            try
            {
                var userResponse = await _userApiRefitClient.CreateUser(request);
                if (userResponse?.Content == null || !userResponse.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to create user via User Service.");
                    return null;
                }

                return userResponse.Content.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user via User API");
                throw;
            }
        }

        public async Task<bool?> IsUserExist(int userId)
        {
            try
            {
                var userResponse = await _userApiRefitClient.GetUser(userId);
                if (userResponse == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return null;
                }

                if (userResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;

                return userResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching user with ID {userId}");
                throw;
            }
        }
    }
}
