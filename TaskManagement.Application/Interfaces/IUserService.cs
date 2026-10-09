using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IUserService
{

    Task<UserDto?> GetByIdAsync(int id);

    Task<UserDto> CreateAsync(CreateUserRequest request);
}