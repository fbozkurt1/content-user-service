namespace ContentService.Domain.Configs
{
    public record ApplicationConfig
    {
        public PostgreSqlServerConfig PostgreSqlServerConfig { get; set; }
        public UserApiConfig UserApiConfig { get; set; }
    }

    public record PostgreSqlServerConfig
    {
        public string ConnectionString { get; set; }
    }

    public record UserApiConfig
    {
        public string BaseUrl { get; set; }
    }
}
