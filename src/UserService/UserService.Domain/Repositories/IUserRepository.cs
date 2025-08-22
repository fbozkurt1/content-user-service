using UserService.Domain.Entities;

namespace UserService.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<int> Add(User user);
        Task<bool> Delete(int id);
        Task<(int TotalCount, IEnumerable<User> Users)> GetAll(int page, int pageSize);
        Task<User?> GetById(int id);
        Task<bool> Update(User user);
    }
}
