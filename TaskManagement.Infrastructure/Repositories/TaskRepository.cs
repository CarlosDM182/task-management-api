using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TaskRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

#region CRUD METHODS
        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            using var connection  = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT 
                Id, 
                Title, 
                Description, 
                DueDate, 
                Priority, 
                Status,
                CreatedAt, 
                UpdatedAt, 
                UserId 
                FROM Tasks;
                """;

            return await connection.QueryAsync<TaskItem>(sql);
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                SELECT 
                Id, 
                Title, 
                Description, 
                DueDate, 
                Priority, 
                Status,
                CreatedAt, 
                UpdatedAt, 
                UserId 
                FROM Tasks
                WHERE Id = @Id;
                """;

            return await connection.QueryFirstOrDefaultAsync<TaskItem>(sql, new   { Id = id });
        }

        public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
        SELECT
            Id,
            Title,
            Description,
            DueDate,
            Priority,
            Status,
            CreatedAt,
            UpdatedAt,
            UserId
        FROM Tasks
        WHERE UserId = @UserId;
        """;

            return await connection.QueryAsync<TaskItem>(
                sql,
                new { UserId = userId });
        }
        public async Task<int> CreateAsync(TaskItem task)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
                INSERT INTO Tasks 
                (
                    Title, 
                    Description, 
                    DueDate, 
                    Priority, 
                    Status, 
                    CreatedAt, 
                    UserId
                )

                OUTPUT INSERTED.Id
                VALUES 
                (
                    @Title, 
                    @Description, 
                    @DueDate, 
                    @Priority, 
                    @Status, 
                    @CreatedAt, 
                    @UserId
                    );
                """;
            return await connection.ExecuteScalarAsync<int>(sql, new
            {
                task.Title,
                task.Description,
                task.DueDate,
                Priority = task.Priority.ToString(),
                Status = task.Status.ToString(),
                task.CreatedAt,
                task.UserId
            });
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
            DELETE FROM Tasks
            WHERE Id = @Id;
            """;

            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<bool> UpdateAsync(TaskItem task)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = """
            UPDATE Tasks
            SET
                Title = @Title,
                Description = @Description,
                DueDate = @DueDate,
                Priority = @Priority,
                Status = @Status,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id;
            """;

            var affectedRows = await connection.ExecuteAsync(sql, new
            {
                task.Title,
                task.Description,
                task.DueDate,
                Priority = task.Priority.ToString(),
                Status = task.Status.ToString(),
                task.UpdatedAt,
                task.Id
            });

            return affectedRows > 0;
        }

        

        #endregion
    }
}
