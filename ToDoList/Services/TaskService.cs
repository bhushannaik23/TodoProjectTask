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

        public async Task<GetTasksResponse> GetTasksAsync(GetTasksRequest request)
        {
            var pagedResult = await _taskRepository.GetTasksAsync(request);

            var response = new GetTasksResponse
            {
                Items = pagedResult.Items.Select(task => new TaskResponse
                {
                    Id = task.TaskItemId,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = task.Status.StatusName
                }),

                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = (int)Math.Ceiling((double)pagedResult.TotalCount / request.PageSize)
            };

            return response;
        }
    }
}
