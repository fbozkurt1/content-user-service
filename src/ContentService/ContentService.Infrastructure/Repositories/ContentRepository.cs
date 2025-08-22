using ContentService.Domain.Configs;
using ContentService.Domain.Entities;
using ContentService.Domain.Repositories;
using ContentService.Infrastructure.Repositories.DataDtos;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ContentService.Infrastructure.Repositories
{
    public class ContentRepository : IContentRepository
    {
        private readonly ApplicationConfig _config;
        private readonly string _connectionString;

        public ContentRepository(IOptions<ApplicationConfig> options)
        {
            _config = options.Value;

            _connectionString = _config.PostgreSqlServerConfig.ConnectionString;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<int> Add(Content content)
        {
            var sql = @"
                    INSERT INTO contents (title, body, user_id, created_at)
                    VALUES (@Title, @Body, @UserId, @CreatedAt)
                    RETURNING id;";

            using var connection = CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                content.Title,
                content.Body,
                content.UserId,
                CreatedAt = DateTime.Now
            });
        }

        public async Task<Content?> GetById(int id)
        {
            var sql = @"SELECT 
                             id, title, body, user_id, created_at, updated_at
                        FROM contents 
                        WHERE id = @Id AND is_deleted=false;";
            using var connection = CreateConnection();
            var content = await connection.QueryFirstOrDefaultAsync<ContentDataDto?>(sql, new { Id = id });
            if (content == null)
                return null;

            return new Content
            {
                Id = content.Id,
                Title = content.Title,
                Body = content.Body,
                UserId = content.UserId,
                CreatedAt = content.CreatedAt,
                UpdatedAt = content.UpdatedAt
            };
        }

        public async Task<(int TotalCount, IEnumerable<Content> Contents)> GetAll(int page, int pageSize)
        {
            if (page < 1)
                page = 1;

            if (pageSize <= 0 || pageSize >= 100)
                pageSize = 20;

            using var connection = CreateConnection();
            var sql = @"SELECT 
                    id,
                    title,
                    body,
                    user_id,
                    created_at,
                    updated_at,
                    COUNT(*) OVER() AS TotalCount
                FROM contents
                WHERE is_deleted = false
                ORDER BY id DESC 
                LIMIT @PageSize OFFSET @Offset;";

            var offset = (page - 1) * pageSize;
            var contentDataDtos = await connection.QueryAsync<ContentWithTotalCountDataDto>(sql, new
            {
                PageSize = pageSize,
                Offset = offset
            });

            var totalCount = contentDataDtos.FirstOrDefault()?.TotalCount ?? 0;
            var contents = contentDataDtos.Select(c => new Content
            {
                Id = c.Id,
                Title = c.Title,
                Body = c.Body,
                UserId = c.UserId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });

            return (totalCount, contents);
        }

        public async Task<bool> Update(Content content)
        {
            var sql = @"
                    UPDATE contents
                    SET 
                        title = @Title, 
                        body = @Body, 
                        user_id = @UserId,
                        updated_at = @UpdatedAt
                    WHERE id = @Id AND is_deleted=false;";

            using var connection = CreateConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, new
            {
                content.Title,
                content.Body,
                content.UserId,
                UpdatedAt = DateTime.UtcNow,
                content.Id
            });
            return rowsAffected > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var sql = @"
                        UPDATE contents
                        SET 
                            is_deleted = true, 
                            updated_at = @UpdatedAt
                        WHERE id = @Id;";

            using var connection = CreateConnection();
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.Now });
            return affectedRows > 0;
        }

        public virtual IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);
    }
}
