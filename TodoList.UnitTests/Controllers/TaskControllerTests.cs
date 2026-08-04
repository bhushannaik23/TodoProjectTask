using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Controllers;
using ToDoList.DTOs;
using ToDoList.Interfaces;

namespace TodoList.UnitTests.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();

            _controller = new TaskController(
                _taskServiceMock.Object);
        }

        [Fact]
        public async Task GetTaskById_ShouldReturnOkWithTask_WhenTaskExists()
        {
            // Arrange
            var taskId = 1;

            var response = new TaskResponse
            {
                Id = taskId,
                Title = "Learn Controller Testing",
                Description = "Testing GET by ID",
                DueDate = DateTime.Today.AddDays(2),
                Status = "Pending"
            };

            _taskServiceMock
                .Setup(x => x.GetTaskByIdAsync(taskId))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetTaskById(taskId);

            // Assert
            var okResult = result.Should()
                .BeOfType<OkObjectResult>()
                .Subject;

            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

            okResult.Value.Should().Be(response);

            _taskServiceMock.Verify(
                x => x.GetTaskByIdAsync(taskId),
                Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedAtAction_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = "Learn Controller Testing",
                Description = "Testing create endpoint",
                DueDate = DateTime.Today.AddDays(2)
            };

            var response = new CreateTaskResponse
            {
                TaskId = 1,
                Message = "Task created successfully."
            };

            _taskServiceMock
                .Setup(x => x.AddAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = result.Should()
                .BeOfType<CreatedAtActionResult>()
                .Subject;

            createdResult.StatusCode.Should()
                .Be(StatusCodes.Status201Created);

            createdResult.Value.Should().Be(response);

            createdResult.ActionName.Should()
                .Be(nameof(TaskController.Create));

            createdResult.RouteValues.Should()
                .ContainKey("id")
                .WhoseValue.Should().Be(response.TaskId);

            _taskServiceMock.Verify(
                x => x.AddAsync(request),
                Times.Once);
        }

        [Fact]
        public async Task GetTasks_ShouldReturnOkWithPagedResponse()
        {
            // Arrange
            var request = new GetTasksRequest
            {
                Page = 1,
                PageSize = 10
            };

            var response = new GetTasksResponse
            {
                Items = new List<TaskResponse>
        {
            new TaskResponse
            {
                Id = 1,
                Title = "Task 1",
                DueDate = DateTime.Today.AddDays(1),
                Status = "Pending"
            }
        },
                Page = 1,
                PageSize = 10,
                TotalCount = 1,
                TotalPages = 1
            };

            _taskServiceMock
                .Setup(x => x.GetTasksAsync(request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetTasks(request);

            // Assert
            var okResult = result.Should()
                .BeOfType<OkObjectResult>()
                .Subject;

            okResult.StatusCode.Should()
                .Be(StatusCodes.Status200OK);

            okResult.Value.Should().Be(response);

            _taskServiceMock.Verify(
                x => x.GetTasksAsync(request),
                Times.Once);
        }

        [Fact]
        public async Task UpdateTask_ShouldReturnNoContent_WhenRequestIsValid()
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

            _taskServiceMock
                .Setup(x => x.UpdateTaskAsync(taskId, request))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateTask(taskId, request);

            // Assert
            var noContentResult = result.Should()
                .BeOfType<NoContentResult>()
                .Subject;

            noContentResult.StatusCode.Should()
                .Be(StatusCodes.Status204NoContent);

            _taskServiceMock.Verify(
                x => x.UpdateTaskAsync(taskId, request),
                Times.Once);
        }
    }
}
