using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IAuthService
{
    Task<string?> LoginAsync(LoginRequest request);
}