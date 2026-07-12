using ToDoList.Constants;
using ToDoList.DTOs;
using ToDoList.Entities;
using ToDoList.Interfaces;

namespace ToDoList.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<CreateTaskResponse> AddAsync(CreateTaskRequest request)
        {
            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate ?? DateTime.Today,
                StatusId = StatusConstants.Pending
            };

            var createdTask = await _taskRepository.AddAsync(task);

            return new CreateTaskResponse
            {
                TaskId = createdTask.TaskItemId,
                Message = "Task created successfully."
            };
        }
    }
}
