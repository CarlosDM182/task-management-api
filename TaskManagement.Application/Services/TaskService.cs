using Microsoft.Extensions.Logging;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using Enums = TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepository, IUserRepository userRepository, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskDto>> GetAllAsync()
        {
            var task = await _taskRepository.GetAllAsync();
            return task.Select(MapToDto);

        }

        public async Task<TaskDto?> GetByIdAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id);

            if (task is null)
            {
                return null;
            }

            if (task.UserId != userId)
            {
                return null;
            }

            return MapToDto(task);
        }

        public async Task<IEnumerable<TaskDto>> GetByUserIdAsync(int userId)
        {
            var tasks = await _taskRepository.GetByUserIdAsync(userId);

            return tasks.Select(MapToDto);
        }

        public async Task<TaskDto> CreateAsync(CreateTaskRequest request, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                throw new KeyNotFoundException(
                    $"User with id {userId} was not found.");
            }

            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                Priority = request.Priority,
                Status = Enums.TaskStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId
            };

            var id = await _taskRepository.CreateAsync(task);
            var createdTask = await _taskRepository.GetByIdAsync(id);
            if (createdTask is null)
                throw new InvalidOperationException("The task was created but could not be retrieved");

            _logger.LogInformation("Task created successfully with ID: {TaskId}, UserId : {UserId}", id, userId);
            return MapToDto(createdTask);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var existingTask = await _taskRepository.GetByIdAsync(id);

            if (existingTask is null)
            {
                return false;
            }

            if (existingTask.UserId != userId)
            {
                return false;
            }

            return await _taskRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTaskRequest request, int userId)
        {
            var existingTask = await _taskRepository.GetByIdAsync(id);

            if (existingTask is null)
            {
                return false;
            }

            if (existingTask.UserId != userId)
            {
                return false;
            }

            existingTask.Title = request.Title;
            existingTask.Description = request.Description;
            existingTask.DueDate = request.DueDate;
            existingTask.Priority = request.Priority;
            existingTask.Status = (Enums.TaskStatus)request.Status;
            existingTask.UpdatedAt = DateTime.UtcNow;

            return await _taskRepository.UpdateAsync(existingTask);
        }


        private static TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                UserId = task.UserId
            };
        }
    }
}
