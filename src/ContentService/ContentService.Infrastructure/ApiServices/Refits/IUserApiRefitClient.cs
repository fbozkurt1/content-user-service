using ContentService.Domain.ApiServices.Dtos;
using ContentService.Infrastructure.ApiServices.Refits.DataDtos;
using Refit;
using System.Threading.Tasks;
namespace ContentService.Infrastructure.ApiServices.Refits
{
    [Headers("Content-Type : application/json")]
    public interface IUserApiRefitClient
    {
        [Post("/v1/users")]
        Task<ApiResponse<UserDataDto>> CreateUser([Body] CreateUserDto request);

        [Get("/v1/users/{userId}")]
        Task<ApiResponse<UserDataDto>> GetUser(int userId);
    }
}
