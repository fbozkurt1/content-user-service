using System;

namespace ContentService.Infrastructure.Repositories.DataDtos
{
    public record ContentDataDto
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Body { get; init; }
        public int UserId { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
