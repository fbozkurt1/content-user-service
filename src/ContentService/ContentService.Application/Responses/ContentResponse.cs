using System;

namespace ContentService.Application.Responses
{
    public class ContentResponse
    {
        public int? Id { get; }
        public string Title { get; }
        public string Body { get; }
        public int UserId { get; }
        public DateTime CreatedAt { get; }
        public DateTime? UpdatedAt { get; }

        public ContentResponse(int? id, string title, string body, int userId, DateTime createdAt, DateTime? updatedAt)
        {
            Id = id;
            Title = title;
            Body = body;
            UserId = userId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public ContentResponse(Domain.Entities.Content content)
        {
            Id = content.Id;
            Title = content.Title;
            Body = content.Body;
            UserId = content.UserId;
            CreatedAt = content.CreatedAt;
            UpdatedAt = content.UpdatedAt;
        }
    }
}
