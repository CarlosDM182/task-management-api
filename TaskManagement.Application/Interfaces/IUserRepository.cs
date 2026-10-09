using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces;

public interface IUserRepository
{

    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);

    Task<int> CreateAsync(User user);

    Task<bool> ExistsByEmailAsync(string email);
}