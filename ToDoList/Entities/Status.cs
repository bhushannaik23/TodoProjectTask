namespace ToDoList.Entities
{
    public class Status
    {
        public int StatusId { get; set; }

        public string StatusName { get; set; } = string.Empty;

        // Navigation Property
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}

