using System;

namespace ContentService.Infrastructure.ApiServices.Refits.DataDtos
{
    public record UserDataDto
    {
        public int Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
