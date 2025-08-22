namespace UserService.Infrastructure.DataDtos
{
    public record UserDataDto
    {
        public int Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}
