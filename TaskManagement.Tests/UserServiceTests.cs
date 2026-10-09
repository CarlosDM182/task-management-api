using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateUser_WhenEmailDoesNotExist()
    {
        // Arrange
        var userRepository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var logger = new Mock<ILogger<UserService>>();

        userRepository
            .Setup(x => x.ExistsByEmailAsync("carlos@test.com"))
            .ReturnsAsync(false);

        passwordHasher
            .Setup(x => x.Hash("MiPassword123"))
            .Returns("hashed-password");

        userRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(1);

        userRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new User
            {
                Id = 1,
                Name = "Carlos",
                Email = "carlos@test.com",
                PasswordHash = "hashed-password",
                CreatedAt = DateTime.UtcNow
            });

        var service = new UserService(
            userRepository.Object,
            logger.Object,
            passwordHasher.Object);

        var request = new CreateUserRequest
        {
            Name = "Carlos",
            Email = "carlos@test.com",
            Password = "MiPassword123"
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Carlos", result.Name);
        Assert.Equal("carlos@test.com", result.Email);

        userRepository.Verify(
            x => x.CreateAsync(
                It.Is<User>(user =>
                    user.Email == "carlos@test.com" &&
                    user.PasswordHash == "hashed-password")),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenEmailAlreadyExists()
    {
        // Arrange
        var userRepository = new Mock<IUserRepository>();
        var passwordHasher = new Mock<IPasswordHasher>();
        var logger = new Mock<ILogger<UserService>>();

        userRepository
            .Setup(x => x.ExistsByEmailAsync("carlos@test.com"))
            .ReturnsAsync(true);

        var service = new UserService(
            userRepository.Object,
            logger.Object,
            passwordHasher.Object
            );

        var request = new CreateUserRequest
        {
            Name = "Carlos",
            Email = "carlos@test.com",
            Password = "MiPassword123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateAsync(request));

        userRepository.Verify(
            x => x.CreateAsync(It.IsAny<User>()),
            Times.Never);
    }
}