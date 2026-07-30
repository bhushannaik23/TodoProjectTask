namespace ToDoList.DTOs
{
    public class GetTasksRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
