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

        public TaskService(ITaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<CreateTaskResponse> AddAsync(CreateTaskRequest request)
        {
            //var task = new TaskItem
            //{
            //    Title = request.Title,
            //    Description = request.Description,
            //    DueDate = request.DueDate ?? DateTime.Today,
            //    StatusId = StatusConstants.Pending
            //};

            var task = _mapper.Map<TaskItem>(request);

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

            return response;
        }

        public async Task<TaskResponse> GetTaskByIdAsync(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task is null)
            {
                throw new KeyNotFoundException($"Task with Id {id} was not found.");
            }

            //return new TaskResponse
            //{
            //    Id = task.TaskItemId,
            //    Title = task.Title,
            //    Description = task.Description,
            //    DueDate = task.DueDate,
            //    Status = task.Status.StatusName
            //};

            return _mapper.Map<TaskResponse>(task);
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _taskRepository.GetTrackedTaskByIdAsync(id);

            if (task is null)
            {
                throw new KeyNotFoundException($"Task with Id {id} was not found.");
            }

            await _taskRepository.DeleteTaskAsync(task);

            await _taskRepository.SaveChangesAsync();
        }

        public async Task UpdateTaskAsync(int id, UpdateTaskRequest request)
        {
            var task = await _taskRepository.GetTrackedTaskByIdAsync(id);

            if (task is null)
            {
                throw new ResourceNotFoundException($"Task with Id {id} was not found.");
            }

            var statusExists = await _taskRepository.StatusExistsAsync(request.StatusId);

            if (!statusExists)
            {
                throw new BadRequestException($"Status with Id {request.StatusId} does not exist.");
            }

            //task.Title = request.Title;
            //task.Description = request.Description;
            //task.DueDate = request.DueDate;
            //task.StatusId = request.StatusId;

            _mapper.Map(request, task);

            await _taskRepository.SaveChangesAsync();
        }
    }
}
