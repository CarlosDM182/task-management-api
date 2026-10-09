using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;

namespace TaskManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITaskService _taskService;

    public UsersController(IUserService userService, ITaskService taskService)
    {
        _userService = userService;
        _taskService = taskService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userIdValue = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

        if(!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }
        if(id != userId)
        {
            return Forbid();
        }

        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            user);
    }

    [HttpGet("{id:int}/tasks")]
    public async Task<IActionResult> GetTasks(int id)
    {
        var userIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out var userId))
        {
            return Unauthorized();
        }

        if (id != userId)
        {
            return Forbid();
        }

        var user = await _userService.GetByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        var tasks = await _taskService.GetByUserIdAsync(id);

        return Ok(tasks);
    }
}