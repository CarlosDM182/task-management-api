using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();

        Task<TaskItem?> GetByIdAsync(int id);

        Task<IEnumerable<TaskItem>> GetByUserIdAsync(int userId);

        Task<int> CreateAsync(TaskItem task);

        Task<bool> UpdateAsync(TaskItem task);

        Task<bool> DeleteAsync(int id);
        
    }
}
