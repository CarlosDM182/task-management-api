using System.ComponentModel.DataAnnotations;
using Enums =  TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs;

public class UpdateTaskRequest
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public Enums.TaskPriority Priority { get; set; }

    public Enums.TaskStatus Status { get; set; }
}