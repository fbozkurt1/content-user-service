namespace ContentService.Infrastructure.Repositories.DataDtos
{
    public record ContentWithTotalCountDataDto : ContentDataDto
    {
        public int TotalCount { get; init; }
    }
}
