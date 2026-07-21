using ToDoList.DTOs;

namespace ToDoList.Interfaces
{
    public interface ITaskService
    {
        Task<CreateTaskResponse> AddAsync(CreateTaskRequest request);

        Task<GetTasksResponse> GetTasksAsync(GetTasksRequest request);

        Task<TaskResponse> GetTaskByIdAsync(int id);

        Task DeleteTaskAsync(int id);
    }
}

