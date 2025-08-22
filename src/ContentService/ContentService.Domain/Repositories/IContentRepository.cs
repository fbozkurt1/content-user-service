using ContentService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentService.Domain.Repositories
{
    public interface IContentRepository
    {
        Task<int> Add(Content content);
        Task<Content?> GetById(int id);
        Task<bool> Update(Content content);
        Task<bool> Delete(int id);
        Task<(int TotalCount, IEnumerable<Content> Contents)> GetAll(int page, int pageSize);
    }
}
