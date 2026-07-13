using ToDoList.Entities;

namespace ToDoList.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskItem> AddAsync(TaskItem task);
    }
}
