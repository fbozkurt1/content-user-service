using ContentService.Application.Requests;
using ContentService.Application.Responses;
using System.Threading.Tasks;

namespace ContentService.Application.Services
{
    public interface IContentService
    {
        Task<ContentResponse> CreateContent(SaveContentRequest request);
        Task<bool> DeleteContent(int contentId);
        Task<ContentListResponse> GetAllContents(int page, int pageSize);
        Task<ContentResponse?> GetContent(int contentId);
        Task<ContentResponse?> UpdateContent(int contentId, SaveContentRequest request);
    }
}
