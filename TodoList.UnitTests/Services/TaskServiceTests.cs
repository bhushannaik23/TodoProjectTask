using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Constants;
using ToDoList.DTOs;
using ToDoList.Models;
using ToDoList.Entities;
using ToDoList.Exceptions;
using ToDoList.Interfaces;
using ToDoList.Services;

namespace TodoList.UnitTests.Services
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TaskService>> _loggerMock;

        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<TaskService>>();

            _taskService = new TaskService(
                _taskRepositoryMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnCreateTaskResponse_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = "Learn Unit Testing",
                Description = "Write first service test",
                DueDate = DateTime.UtcNow.AddDays(2)
            };

            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate ?? DateTime.Today,
                StatusId = StatusConstants.Pending
            };

            var createdTask = new TaskItem
            {
                TaskItemId = 1,
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate ?? DateTime.Today,
                StatusId = StatusConstants.Pending
            };

            _mapperMock
                .Setup(x => x.Map<TaskItem>(request))
                .Returns(task);

            _taskRepositoryMock
                .Setup(x => x.AddAsync(task))
                .ReturnsAsync(createdTask);

            // Act
            var result = await _taskService.AddAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.TaskId.Should().Be(1);
            result.Message.Should().Be("Task created successfully.");

            _mapperMock.Verify(
                x => x.Map<TaskItem>(request),
                Times.Once);

            _taskRepositoryMock.Verify(
                x => x.AddAsync(task),
                Times.Once);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldThrowResourceNotFoundException_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = 999;

            _taskRepositoryMock
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = async () =>
                await _taskService.GetTaskByIdAsync(taskId);

            // Assert
            await act.Should()
                .ThrowAsync<ResourceNotFoundException>()
                .WithMessage($"Task with Id {taskId} was not found.");

            _taskRepositoryMock.Verify(
                x => x.GetTaskByIdAsync(taskId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<TaskResponse>(It.IsAny<TaskItem>()),
                Times.Never);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ShouldReturnTaskResponse_WhenTaskExists()
        {
            // Arrange
            var taskId = 1;

            var task = new TaskItem
            {
                TaskItemId = taskId,
                Title = "Learn Unit Testing",
                Description = "Testing GetById",
                DueDate = DateTime.Today.AddDays(2),
                StatusId = StatusConstants.Pending
            };

            var expectedResponse = new TaskResponse
            {
                Id = taskId,
                Title = "Learn Unit Testing",
                Description = "Testing GetById",
                DueDate = task.DueDate,
                Status = "Pending"
            };

            _taskRepositoryMock
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            _mapperMock
                .Setup(x => x.Map<TaskResponse>(task))
                .Returns(expectedResponse);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId);

            // Assert
            result.Should().NotBeNull();

            result.Id.Should().Be(taskId);
            result.Title.Should().Be("Learn Unit Testing");
            result.Status.Should().Be("Pending");

            _taskRepositoryMock.Verify(
                x => x.GetTaskByIdAsync(taskId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<TaskResponse>(task),
                Times.Once);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldThrowResourceNotFoundException_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = 999;

            var request = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                DueDate = DateTime.Today.AddDays(2),
                StatusId = 1
            };

            _taskRepositoryMock
                .Setup(x => x.GetTrackedTaskByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = async () =>
                await _taskService.UpdateTaskAsync(taskId, request);

            // Assert
            await act.Should()
                .ThrowAsync<ResourceNotFoundException>()
                .WithMessage($"Task with Id {taskId} was not found.");

            _taskRepositoryMock.Verify(
                x => x.GetTrackedTaskByIdAsync(taskId),
                Times.Once);

            _taskRepositoryMock.Verify(
                x => x.StatusExistsAsync(It.IsAny<int>()),
                Times.Never);

            _taskRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldThrowBadRequestException_WhenStatusDoesNotExist()
        {
            // Arrange
            var taskId = 1;

            var request = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                DueDate = DateTime.Today.AddDays(2),
                StatusId = 999
            };

            var task = new TaskItem
            {
                TaskItemId = taskId,
                Title = "Old Task",
                Description = "Old Description",
                DueDate = DateTime.Today.AddDays(1),
                StatusId = 1
            };

            _taskRepositoryMock
                .Setup(x => x.GetTrackedTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            _taskRepositoryMock
                .Setup(x => x.StatusExistsAsync(request.StatusId))
                .ReturnsAsync(false);

            // Act
            Func<Task> act = async () =>
                await _taskService.UpdateTaskAsync(taskId, request);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage($"Status with Id {request.StatusId} does not exist.");

            _taskRepositoryMock.Verify(
                x => x.GetTrackedTaskByIdAsync(taskId),
                Times.Once);

            _taskRepositoryMock.Verify(
                x => x.StatusExistsAsync(request.StatusId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map(request, task),
                Times.Never);

            _taskRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task UpdateTaskAsync_ShouldUpdateTask_WhenRequestIsValid()
        {
            // Arrange
            var taskId = 1;

            var request = new UpdateTaskRequest
            {
                Title = "Updated Task",
                Description = "Updated Description",
                DueDate = DateTime.Today.AddDays(5),
                StatusId = 2
            };

            var task = new TaskItem
            {
                TaskItemId = taskId,
                Title = "Old Task",
                Description = "Old Description",
                DueDate = DateTime.Today.AddDays(1),
                StatusId = 1
            };

            _taskRepositoryMock
                .Setup(x => x.GetTrackedTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            _taskRepositoryMock
                .Setup(x => x.StatusExistsAsync(request.StatusId))
                .ReturnsAsync(true);

            _taskRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _taskService.UpdateTaskAsync(taskId, request);

            // Assert
            _taskRepositoryMock.Verify(
                x => x.GetTrackedTaskByIdAsync(taskId),
                Times.Once);

            _taskRepositoryMock.Verify(
                x => x.StatusExistsAsync(request.StatusId),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map(request, task),
                Times.Once);

            _taskRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldThrowResourceNotFoundException_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = 999;

            _taskRepositoryMock
                .Setup(x => x.GetTrackedTaskByIdAsync(taskId))
                .ReturnsAsync((TaskItem?)null);

            // Act
            Func<Task> act = async () =>
                await _taskService.DeleteTaskAsync(taskId);

            // Assert
            await act.Should()
                .ThrowAsync<ResourceNotFoundException>()
                .WithMessage($"Task with Id {taskId} was not found.");

            _taskRepositoryMock.Verify(
                x => x.GetTrackedTaskByIdAsync(taskId),
                Times.Once);

            _taskRepositoryMock.Verify(
                x => x.DeleteTaskAsync(It.IsAny<TaskItem>()),
                Times.Never);

            _taskRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }

        [Fact]
        public async Task DeleteTaskAsync_ShouldDeleteTask_WhenTaskExists()
        {
            // Arrange
            var taskId = 1;

            var task = new TaskItem
            {
                TaskItemId = taskId,
                Title = "Task to delete",
                Description = "This task will be deleted",
                DueDate = DateTime.Today.AddDays(2),
                StatusId = StatusConstants.Pending
            };

            _taskRepositoryMock
                .Setup(x => x.GetTrackedTaskByIdAsync(taskId))
                .ReturnsAsync(task);

            _taskRepositoryMock
                .Setup(x => x.DeleteTaskAsync(task))
                .Returns(Task.CompletedTask);

            _taskRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _taskService.DeleteTaskAsync(taskId);

            // Assert
            _taskRepositoryMock.Verify(
                x => x.GetTrackedTaskByIdAsync(taskId),
                Times.Once);

            _taskRepositoryMock.Verify(
                x => x.DeleteTaskAsync(task),
                Times.Once);

            _taskRepositoryMock.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnPagedTasks_WhenTasksExist()
        {
            // Arrange
            var request = new GetTasksRequest
            {
                Page = 2,
                PageSize = 2
            };

            var tasks = new List<TaskItem>
    {
        new TaskItem
        {
            TaskItemId = 3,
            Title = "Task 3",
            DueDate = DateTime.Today.AddDays(1),
            StatusId = StatusConstants.Pending
        },
        new TaskItem
        {
            TaskItemId = 4,
            Title = "Task 4",
            DueDate = DateTime.Today.AddDays(2),
            StatusId = StatusConstants.Pending
        }
    };

            var mappedTasks = new List<TaskResponse>
    {
        new TaskResponse
        {
            Id = 3,
            Title = "Task 3",
            DueDate = tasks[0].DueDate,
            Status = "Pending"
        },
        new TaskResponse
        {
            Id = 4,
            Title = "Task 4",
            DueDate = tasks[1].DueDate,
            Status = "Pending"
        }
    };

            var pagedResult = new PagedTaskResult
            {
                Items = tasks,
                TotalCount = 5
            };

            _taskRepositoryMock
                .Setup(x => x.GetTasksAsync(request))
                .ReturnsAsync(pagedResult);

            _mapperMock
                .Setup(x => x.Map<IEnumerable<TaskResponse>>(tasks))
                .Returns(mappedTasks);

            // Act
            var result = await _taskService.GetTasksAsync(request);

            // Assert
            result.Should().NotBeNull();

            result.Items.Should().HaveCount(2);

            result.Page.Should().Be(2);
            result.PageSize.Should().Be(2);

            result.TotalCount.Should().Be(5);

            result.TotalPages.Should().Be(3);

            _taskRepositoryMock.Verify(
                x => x.GetTasksAsync(request),
                Times.Once);

            _mapperMock.Verify(
                x => x.Map<IEnumerable<TaskResponse>>(tasks),
                Times.Once);
        }

        [Fact]
        public async Task GetTasksAsync_ShouldReturnEmptyItems_WhenNoTasksExist()
        {
            // Arrange
            var request = new GetTasksRequest
            {
                Page = 1,
                PageSize = 10
            };

            var tasks = Enumerable.Empty<TaskItem>();

            var pagedResult = new PagedTaskResult
            {
                Items = tasks,
                TotalCount = 0
            };

            _taskRepositoryMock
                .Setup(x => x.GetTasksAsync(request))
                .ReturnsAsync(pagedResult);

            _mapperMock
                .Setup(x => x.Map<IEnumerable<TaskResponse>>(tasks))
                .Returns(Enumerable.Empty<TaskResponse>());

            // Act
            var result = await _taskService.GetTasksAsync(request);

            // Assert
            result.Should().NotBeNull();

            result.Items.Should().BeEmpty();
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalCount.Should().Be(0);
            result.TotalPages.Should().Be(0);

            _taskRepositoryMock.Verify(
                x => x.GetTasksAsync(request),
                Times.Once);
        }

    }
}
