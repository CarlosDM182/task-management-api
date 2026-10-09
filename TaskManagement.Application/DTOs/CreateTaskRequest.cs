using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs;

public class CreateTaskRequest
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

}