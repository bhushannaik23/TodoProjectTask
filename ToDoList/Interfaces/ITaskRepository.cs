using ToDoList.DTOs;
using ToDoList.Entities;
using ToDoList.Models;

namespace ToDoList.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem> AddAsync(TaskItem task);

        Task<PagedTaskResult> GetTasksAsync(GetTasksRequest request);

        Task<TaskItem?> GetTaskByIdAsync(int id);

        Task<TaskItem?> GetTrackedTaskByIdAsync(int id);

        Task DeleteTaskAsync(TaskItem task);

        Task SaveChangesAsync();
    }
}
