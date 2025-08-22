namespace UserService.Infrastructure.DataDtos
{
    public record UserWithTotalCountDataDto : UserDataDto
    {
        public int TotalCount { get; init; }
    }
}
