using UserService.Application.Requests;
using UserService.Application.Responses;

namespace UserService.Application.Services
{
    public interface IUserService
    {
        Task<UserResponse> CreateUser(SaveUserRequest request);
        Task<bool> DeleteUser(int userId);
        Task<UserListResponse> GetAllUsers(int page, int pageSize);
        Task<UserResponse?> GetUser(int userId);
        Task<UserResponse?> UpdateUser(int userId, SaveUserRequest request);
    }
}
