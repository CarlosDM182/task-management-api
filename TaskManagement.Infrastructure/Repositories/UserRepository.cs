using Dapper;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            SELECT
                Id,
                Name,
                Email,
                CreatedAt
            FROM Users
            WHERE Id = @Id;
            """;

        return await connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
        SELECT
            Id,
            Name,
            Email,
            PasswordHash,
            CreatedAt
        FROM Users
        WHERE Email = @Email;
        """;

        return await connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { Email = email });
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
            INSERT INTO Users
            (
                Name,
                Email,
                PasswordHash,
                CreatedAt
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @Name,
                @Email,
                @PasswordHash,
                @CreatedAt
            );
            """;

        return await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                user.Name,
                user.Email,
                user.PasswordHash,
                user.CreatedAt
            });
    }
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string sql = """
        SELECT COUNT(1)
        FROM Users
        WHERE Email = @Email;
        """;

        var count = await connection.ExecuteScalarAsync<int>(
            sql,
            new { Email = email });

        return count > 0;
    }
}