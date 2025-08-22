using System.Collections.Generic;

namespace ContentService.Application.Responses
{
    public class ContentListResponse
    {
        public int TotalCount { get; set; }
        public IEnumerable<ContentResponse> Contents { get; set; }

        public ContentListResponse(int totalCount, IEnumerable<ContentResponse> contents)
        {
            TotalCount = totalCount;
            Contents = contents;
        }
    }
}
