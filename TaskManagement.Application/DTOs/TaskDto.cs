using TaskManagement.Domain.Enums;
using Enums = TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs;

public class TaskDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public TaskPriority Priority { get; set; }

    public Enums.TaskStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int UserId { get; set; }
}