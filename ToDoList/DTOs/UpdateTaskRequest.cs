namespace ToDoList.DTOs
{
    public class UpdateTaskRequest
    {
        public required string Title { get; set; }

        public string? Description { get; set; }

        public DateTime DueDate { get; set; }

        public int StatusId { get; set; }
    }
}
