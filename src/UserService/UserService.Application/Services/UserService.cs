using Microsoft.Extensions.Logging;
using UserService.Application.Requests;
using UserService.Application.Responses;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Exceptions;

namespace UserService.Application.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserResponse?> GetUser(int userId)
        {
            var user = await _userRepository.GetById(userId);
            if (user == null)
                return null;
            return new UserResponse(user);
        }

        public async Task<UserResponse> CreateUser(SaveUserRequest request)
        {
            var user = new Domain.Entities.User
            {
                Username = request.Username,
                Email = request.Email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var userId = await _userRepository.Add(user);
            if (userId <= 0)
            {
                _logger.LogError("Failed to create user.");
                throw new CustomNotificationException("User creation failed.");
            }
            user.Id = userId;
            return new UserResponse(user);
        }

        public async Task<UserResponse?> UpdateUser(int userId, SaveUserRequest request)
        {
            var existingUser = await _userRepository.GetById(userId);
            if (existingUser == null)
                return null;

            existingUser.Username = request.Username;
            existingUser.Email = request.Email;
            existingUser.UpdatedAt = DateTime.Now;

            var isUpdated = await _userRepository.Update(existingUser);
            if (!isUpdated)
                return null;

            return new UserResponse(existingUser);
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var isDeleted = await _userRepository.Delete(userId);
            if (!isDeleted)
            {
                _logger.LogError($"Failed to delete user with ID {userId}");
                return false;
            }
            return true;
        }

        public async Task<UserListResponse> GetAllUsers(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var (totalCount, users) = await _userRepository.GetAll(page, pageSize);
            if (users == null || !users.Any())
                return new UserListResponse([], totalCount);

            var userResponses = users.Select(u => new UserResponse(u));
            return new UserListResponse(userResponses, totalCount);
        }
    }
}
