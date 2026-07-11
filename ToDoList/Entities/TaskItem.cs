namespace ToDoList.Entities
{
    public class TaskItem
    {
        public int TaskItemId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime DueDate { get; set; }

        public int StatusId { get; set; }

        // Navigation Property
        public Status Status { get; set; } = null!;
    }
}
