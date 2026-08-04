using AutoMapper;
using ToDoList.Constants;
using ToDoList.DTOs;
using ToDoList.Entities;
using ToDoList.Exceptions;
using ToDoList.Interfaces;

namespace ToDoList.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        private readonly IMapper _mapper;

        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository taskRepository, IMapper mapper, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CreateTaskResponse> AddAsync(CreateTaskRequest request)
        {

            _logger.LogInformation(
             "Creating task with title '{Title}' and status ID {DueDate}.",
             request.Title,
             request.DueDate);

            //var task = new TaskItem
            //{
            //    Title = request.Title,
            //    Description = request.Description,
            //    DueDate = request.DueDate ?? DateTime.Today,
            //    StatusId = StatusConstants.Pending
            //};

            var task = _mapper.Map<TaskItem>(request);

            var createdTask = await _taskRepository.AddAsync(task);

            _logger.LogInformation(
            "Task created successfully with ID {TaskId}.",
             task.TaskItemId);

            return new CreateTaskResponse
            {
                TaskId = createdTask.TaskItemId,
                Message = "Task created successfully."
            };
        }

        public async Task<GetTasksResponse> GetTasksAsync(GetTasksRequest request)
        {
            _logger.LogInformation(
             "Retrieving tasks. Page: {Page}, PageSize: {PageSize}.",
              request.Page,
              request.PageSize);

            var pagedResult = await _taskRepository.GetTasksAsync(request);

            var response = new GetTasksResponse { 
            //{
            //    Items = pagedResult.Items.Select(task => new TaskResponse
            //    {
            //        Id = task.TaskItemId,
            //        Title = task.Title,
            //        Description = task.Description,
            //        DueDate = task.DueDate,
            //        Status = task.Status.StatusName
            //    }),

                Items = _mapper.Map<IEnumerable<TaskResponse>>(pagedResult.Items),

                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = pagedResult.TotalCount,
                TotalPages = (int)Math.Ceiling((double)pagedResult.TotalCount / request.PageSize)
            };

            _logger.LogInformation(
           "Retrieved {TaskCount} tasks.",
            response.Items.Count());

            return response;
        }

        public async Task<TaskResponse> GetTaskByIdAsync(int id)
        {
            _logger.LogInformation(
                "Retrieving task with ID {TaskId}.",
                 id);

            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task is null)
            {
                _logger.LogWarning(
                 "Task with ID {TaskId} was not found.",
                 id);

                throw new ResourceNotFoundException($"Task with Id {id} was not found.");
            }

            //return new TaskResponse
            //{
            //    Id = task.TaskItemId,
            //    Title = task.Title,
            //    Description = task.Description,
            //    DueDate = task.DueDate,
            //    Status = task.Status.StatusName
            //};

            _logger.LogInformation(
              "Task with ID {TaskId} retrieved successfully.",
              id);

            return _mapper.Map<TaskResponse>(task);
        }

        public async Task DeleteTaskAsync(int id)
        {
            _logger.LogInformation(
            "Deleting task with ID {TaskId}.",
            id);

            var task = await _taskRepository.GetTrackedTaskByIdAsync(id);

            if (task is null)
            {
                _logger.LogWarning(
                "Task with ID {TaskId} was not found for deletion.",
                id);

                throw new ResourceNotFoundException($"Task with Id {id} was not found.");
            }

            await _taskRepository.DeleteTaskAsync(task);

            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation(
            "Task with ID {TaskId} deleted successfully.",
            id);
        }

        public async Task UpdateTaskAsync(int id, UpdateTaskRequest request)
        {
            _logger.LogInformation(
            "Updating task with ID {TaskId}.",
            id);

            var task = await _taskRepository.GetTrackedTaskByIdAsync(id);

            if (task is null)
            {
                throw new ResourceNotFoundException($"Task with Id {id} was not found.");
            }

            var statusExists = await _taskRepository.StatusExistsAsync(request.StatusId);

            if (!statusExists)
            {
                _logger.LogWarning(
                "Invalid status ID {StatusId} provided while updating task {TaskId}.",
                 request.StatusId,
                 id);

                throw new BadRequestException($"Status with Id {request.StatusId} does not exist.");
            }

            //task.Title = request.Title;
            //task.Description = request.Description;
            //task.DueDate = request.DueDate;
            //task.StatusId = request.StatusId;

            _mapper.Map(request, task);

            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation(
            "Task with ID {TaskId} updated successfully.",
            id);
        }
    }
}
