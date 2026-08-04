using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ToDoList.DTOs;
using Xunit;
using FluentAssertions;

namespace TodoList.IntegrationTests.Controllers
{
    public class TaskApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TaskApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateTask_ShouldReturnCreated_WhenRequestIsValid()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = "Integration Test Task",
                Description = "Created through real HTTP request",
                DueDate = DateTime.Today.AddDays(2)
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                "/api/task",
                request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content
                .ReadFromJsonAsync<CreateTaskResponse>();

            result.Should().NotBeNull();
            result!.TaskId.Should().BeGreaterThan(0);
            result.Message.Should().Be("Task created successfully.");
        }

        [Fact]
        public async Task CreateTask_ShouldReturnBadRequest_WhenRequestIsInvalid()
        {
            // Arrange
            var request = new CreateTaskRequest
            {
                Title = "",
                Description = "Invalid task",
                DueDate = DateTime.Today.AddDays(2)
            };

            // Act
            var response = await _client.PostAsJsonAsync(
                "/api/task",
                request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetTaskById_ShouldReturnTask_WhenTaskExists()
        {
            // Arrange
            var createRequest = new CreateTaskRequest
            {
                Title = "Get Integration Task",
                Description = "Testing create and get together",
                DueDate = DateTime.Today.AddDays(3)
            };

            var createResponse = await _client.PostAsJsonAsync(
                "/api/task",
                createRequest);

            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdTask = await createResponse.Content
                .ReadFromJsonAsync<CreateTaskResponse>();

            createdTask.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync(
                $"/api/task/{createdTask!.TaskId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var task = await response.Content
                .ReadFromJsonAsync<TaskResponse>();

            task.Should().NotBeNull();

            task!.Id.Should().Be(createdTask.TaskId);
            task.Title.Should().Be(createRequest.Title);
            task.Description.Should().Be(createRequest.Description);
        }

        [Fact]
        public async Task GetTaskById_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var taskId = 999999;

            // Act
            var response = await _client.GetAsync(
                $"/api/task/{taskId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteTask_ShouldRemoveTask_WhenTaskExists()
        {
            // Arrange
            var createRequest = new CreateTaskRequest
            {
                Title = "Task To Delete",
                Description = "Integration delete test",
                DueDate = DateTime.Today.AddDays(1)
            };

            var createResponse = await _client.PostAsJsonAsync(
                "/api/task",
                createRequest);

            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdTask = await createResponse.Content
                .ReadFromJsonAsync<CreateTaskResponse>();

            createdTask.Should().NotBeNull();

            // Act
            var deleteResponse = await _client.DeleteAsync(
                $"/api/task/{createdTask!.TaskId}");

            // Assert - DELETE
            deleteResponse.StatusCode
                .Should()
                .Be(HttpStatusCode.NoContent);

            // Assert - make sure it is really gone
            var getResponse = await _client.GetAsync(
                $"/api/task/{createdTask.TaskId}");

            getResponse.StatusCode
                .Should()
                .Be(HttpStatusCode.NotFound);
        }
    }
}
