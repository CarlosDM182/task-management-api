using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllAsync();

    Task<TaskDto?> GetByIdAsync(int id, int userId);
    Task<IEnumerable<TaskDto>> GetByUserIdAsync(int userId);

    Task<TaskDto> CreateAsync(CreateTaskRequest request, int userId);

    Task<bool> UpdateAsync(int id, UpdateTaskRequest request, int userId);

    Task<bool> DeleteAsync(int id, int userId);
    
}