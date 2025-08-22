using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;
using UserService.Domain.Configs;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Infrastructure.DataDtos;

namespace UserService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationConfig _config;
        private readonly string _connectionString;

        public UserRepository(IOptions<ApplicationConfig> options)
        {
            _config = options.Value;

            _connectionString = _config.PostgreSqlServerConfig.ConnectionString;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<User?> GetById(int id)
        {
            var sql = @"SELECT 
                             id, username, email, created_at, updated_at
                        FROM users WHERE id = @Id AND is_deleted=false;";

            using var connection = CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<UserDataDto?>(sql, new { Id = id });
            if (user == null)
                return null;

            return new User
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<(int TotalCount, IEnumerable<User> Users)> GetAll(int page, int pageSize)
        {
            var sql = @"
                        SELECT 
                            id, 
                            username, 
                            email, 
                            created_at, 
                            updated_at,
                            COUNT(*) OVER() AS TotalCount
                        FROM users
                        WHERE is_deleted = false
                        ORDER BY id DESC
                        LIMIT @PageSize OFFSET @Offset;";

            if (page < 1)
                page = 1;
            if (pageSize <= 0)
                pageSize = 20;
            if (pageSize > 100)
                pageSize = 100;

            using var connection = CreateConnection();
            var offset = (page - 1) * pageSize;
            var userDataDtos = await connection.QueryAsync<UserWithTotalCountDataDto>(sql, new { PageSize = pageSize, Offset = offset });
            if (userDataDtos == null || !userDataDtos.Any())
                return (0, Enumerable.Empty<User>());

            var totalCount = userDataDtos.First().TotalCount;
            var users = userDataDtos.Select(u => new User
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            });
            return (totalCount, users);

        }

        public async Task<int> Add(User user)
        {
            var sql = @"
                        INSERT INTO users (username, email, created_at)
                        VALUES (@Username, @Email, @CreatedAt)
                        RETURNING id;
                         ";

            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                user.Username,
                user.Email,
                CreatedAt = DateTime.UtcNow,
            });
        }

        public async Task<bool> Update(User user)
        {
            var sql = @"
                        UPDATE users
                        SET 
                            username = @Username, 
                            email = @Email, 
                            updated_at = @UpdatedAt
                        WHERE id = @Id AND is_deleted=false;";

            using var connection = CreateConnection();
            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                user.Id,
                user.Username,
                user.Email,
                UpdatedAt = DateTime.Now
            });

            return affectedRows > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var sql = @"
                        UPDATE users
                        SET 
                            is_deleted = true, 
                            updated_at = @UpdatedAt
                        WHERE id = @Id;";

            using var connection = CreateConnection();
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.Now });
            return affectedRows > 0;
        }

        private NpgsqlConnection CreateConnection() => new(_connectionString);
    }
}
