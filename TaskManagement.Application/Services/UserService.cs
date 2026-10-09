using Microsoft.Extensions.Logging;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        ILogger<UserService> logger,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request)
    {
        var emailExists = await _userRepository.ExistsByEmailAsync(
        request.Email);

        if (emailExists)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        var id = await _userRepository.CreateAsync(user);

        var createdUser = await _userRepository.GetByIdAsync(id);

        if (createdUser is null)
        {
            throw new InvalidOperationException(
                "The user was created but could not be retrieved.");
        }

        _logger.LogInformation(
            "User created successfully. UserId: {UserId}",
            id);

        return MapToDto(createdUser);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}