using ContentService.Domain.ApiServices.Dtos;
using System.Threading.Tasks;

namespace ContentService.Domain.ApiServices
{
    public interface IUserApiService
    {
        Task<int?> CreateUser(CreateUserDto request);
        Task<bool?> IsUserExist(int userId);
    }
}
