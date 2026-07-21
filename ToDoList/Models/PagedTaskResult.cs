using ToDoList.Entities;

namespace ToDoList.Models
{
    public class PagedTaskResult
    {
        public IEnumerable<TaskItem> Items { get; set; } = Enumerable.Empty<TaskItem>();

        public int TotalCount { get; set; }
    }
}
