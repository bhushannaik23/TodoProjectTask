using ToDoList.DTOs;
using ToDoList.Entities;
using ToDoList.Models;

namespace ToDoList.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem> AddAsync(TaskItem task);

        Task<PagedTaskResult> GetTasksAsync(GetTasksRequest request);
    }
}
