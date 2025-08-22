namespace UserService.Domain.Configs
{
    public record ApplicationConfig
    {
        public PostgreSqlServerConfig PostgreSqlServerConfig { get; set; }
    }

    public record PostgreSqlServerConfig
    {
        public string ConnectionString { get; set; }
    }
}
