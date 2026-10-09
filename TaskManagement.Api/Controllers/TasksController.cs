using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var tasks = await _taskService.GetByUserIdAsync(userId);

        return Ok(tasks);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var task = await _taskService.GetByIdAsync(id, userId);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }
        var task = await _taskService.CreateAsync(request, userId);

        return CreatedAtAction(
            nameof(GetById),
            new { id  = task.Id},
            task
        );

    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
    int id,
    [FromBody] UpdateTaskRequest request
    )
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }
        var updated = await _taskService.UpdateAsync(id, request, userId);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        var deleted = await _taskService.DeleteAsync(
            id,
            userId);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }


    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return Ok(new
        {
            userId
        });
    }
}