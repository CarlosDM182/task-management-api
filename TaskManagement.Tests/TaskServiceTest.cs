using Microsoft.Extensions.Logging;
using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Tests
{
    public class TaskServiceTest
    {
        [Fact]
        public async Task CreateAsync_ShouldCreateTask_WhenUserExists()
        {
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            var user = new User
            {
                Id = 1,
                Name = "Carlos",
                Email = "carlos@test.com"
            };

            userRepository
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(user);

            taskRepository
            .Setup(x => x.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(10);

            taskRepository
          .Setup(x => x.GetByIdAsync(10))
          .ReturnsAsync(new TaskItem
          {
              Id = 10,
              Title = "Aprender Dapper",
              Description = "Crear pruebas",
              Priority = TaskPriority.High,
              Status = Domain.Enums.TaskStatus.Pending,
              UserId = 1,
              CreatedAt = DateTime.UtcNow
          });

            var service = new TaskService(
            taskRepository.Object,
            userRepository.Object,
            logger.Object);

            var request = new CreateTaskRequest
            {
                Title = "Aprender Dapper",
                Description = "Crear pruebas",
                Priority = TaskPriority.High
            };

            var result = await service.CreateAsync(request, 1);

            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("Aprender Dapper", result.Title);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
        {
            
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            userRepository
                .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User?)null);

            var service = new TaskService(
                taskRepository.Object,
                userRepository.Object,
                logger.Object);

            var request = new CreateTaskRequest
            {
                Title = "Tarea inválida",
                Description = "El usuario no existe",
                Priority = TaskPriority.High
            };

            
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => service.CreateAsync(request, 999));
        }

        [Fact]
        public async Task CreateAsync_ShouldAssignAuthenticatedUserId()
        {
            
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            userRepository
                .Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(new User
                {
                    Id = 5,
                    Name = "Usuario",
                    Email = "usuario@test.com"
                });

            taskRepository
                .Setup(x => x.CreateAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(20);

            taskRepository
                .Setup(x => x.GetByIdAsync(20))
                .ReturnsAsync(new TaskItem
                {
                    Id = 20,
                    Title = "Tarea de prueba",
                    Priority = TaskPriority.Medium,
                    Status = Domain.Enums.TaskStatus.Pending,
                    UserId = 5,
                    CreatedAt = DateTime.UtcNow
                });

            var service = new TaskService(
                taskRepository.Object,
                userRepository.Object,
                logger.Object);

            var request = new CreateTaskRequest
            {
                Title = "Tarea de prueba",
                Priority = TaskPriority.Medium
            };

            await service.CreateAsync(request, 5);

            
            taskRepository.Verify(
                x => x.CreateAsync(
                    It.Is<TaskItem>(task => task.UserId == 5)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            taskRepository
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((TaskItem?)null);

            var service = new TaskService(
                taskRepository.Object,
                userRepository.Object,
                logger.Object);

            var request = new UpdateTaskRequest
            {
                Title = "Tarea actualizada",
                Description = "Descripción actualizada",
                Priority = TaskPriority.High,
                Status = Domain.Enums.TaskStatus.InProgress
            };

            
            var result = await service.UpdateAsync(999, request,5);

            Assert.False(result);

            taskRepository.Verify(
                x => x.UpdateAsync(It.IsAny<TaskItem>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTask_WhenTaskExists()
        {
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            var existingTask = new TaskItem
            {
                Id = 10,
                Title = "Título anterior",
                Description = "Descripción anterior",
                Priority = TaskPriority.Low,
                Status = Domain.Enums.TaskStatus.Pending,
                UserId = 5,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            taskRepository
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(existingTask);

            taskRepository
                .Setup(x => x.UpdateAsync(It.IsAny<TaskItem>()))
                .ReturnsAsync(true);

            var service = new TaskService(
                taskRepository.Object,
                userRepository.Object,
                logger.Object);

            var request = new UpdateTaskRequest
            {
                Title = "Título actualizado",
                Description = "Descripción actualizada",
                Priority = TaskPriority.High,
                Status = Domain.Enums.TaskStatus.InProgress
            };

            
            var result = await service.UpdateAsync(10, request,5);

           
            Assert.True(result);

            taskRepository.Verify(
                x => x.UpdateAsync(
                    It.Is<TaskItem>(task =>
                        task.Id == 10 &&
                        task.Title == "Título actualizado" &&
                        task.Description == "Descripción actualizada" &&
                        task.Priority == TaskPriority.High &&
                        task.Status == Domain.Enums.TaskStatus.InProgress &&
                        task.UserId == 5)),
                Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            taskRepository
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((TaskItem?)null);

            var service = new TaskService(
                taskRepository.Object,
                userRepository.Object,
                logger.Object);

            
            var result = await service.DeleteAsync(999,5);

           
            Assert.False(result);

            taskRepository.Verify(
                x => x.DeleteAsync(999),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenTaskExists()
        {
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            taskRepository
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new TaskItem
                {
                    Id = 10,
                    Title = "Tarea a eliminar",
                    UserId = 5
                });

            taskRepository
                .Setup(x => x.DeleteAsync(10))
                .ReturnsAsync(true);

            var service = new TaskService(
                taskRepository.Object,
                userRepository.Object,
                logger.Object);

            var result = await service.DeleteAsync(10,5);

            
            Assert.True(result);

            taskRepository.Verify(
                x => x.DeleteAsync(10),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenTaskBelongsToAnotherUser()
        {
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            var existingTask = new TaskItem
            {
                Id = 10,
                Title = "Tarea de otro usuario",
                UserId = 5
            };

            taskRepository
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(existingTask);

            var service = new TaskService(
                taskRepository.Object,
                userRepository.Object,
                logger.Object);

            var request = new UpdateTaskRequest
            {
                Title = "Intento de modificación",
                Priority = TaskPriority.High,
                Status = Domain.Enums.TaskStatus.InProgress
            };

            var result = await service.UpdateAsync(
                10,
                request,
                99);

            Assert.False(result);

            taskRepository.Verify(
                x => x.UpdateAsync(It.IsAny<TaskItem>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenTaskBelongsToAnotherUser()
        {
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            taskRepository
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new TaskItem
                {
                    Id = 10,
                    Title = "Tarea de otro usuario",
                    UserId = 5
                });

            var service = new TaskService(
                taskRepository.Object,
                userRepository.Object,
                logger.Object);

           
            var result = await service.DeleteAsync(10, 99);

           
            Assert.False(result);

            taskRepository.Verify(
                x => x.DeleteAsync(10),
                Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTask_WhenTaskBelongsToUser()
        {
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            taskRepository
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new TaskItem
                {
                    Id = 10,
                    Title = "Mi tarea",
                    UserId = 5
                });

            var service = new TaskService(
                taskRepository.Object,
                userRepository.Object,
                logger.Object);

           
            var result = await service.GetByIdAsync(10, 5);

           
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("Mi tarea", result.Title);
            Assert.Equal(5, result.UserId);
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenTaskBelongsToAnotherUser()
        {
            var taskRepository = new Mock<ITaskRepository>();
            var userRepository = new Mock<IUserRepository>();
            var logger = new Mock<ILogger<TaskService>>();

            taskRepository
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(new TaskItem
                {
                    Id = 10,
                    Title = "Tarea de otro usuario",
                    UserId = 5
                });

            var service = new TaskService(
                taskRepository.Object,
                userRepository.Object,
                logger.Object);

            
            var result = await service.GetByIdAsync(10, 99);

            
            Assert.Null(result);
        }
    }


}
