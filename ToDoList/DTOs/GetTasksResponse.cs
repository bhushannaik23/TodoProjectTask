namespace ToDoList.DTOs
{
    public class GetTasksResponse
    {
        public IEnumerable<TaskResponse> Items { get; set; } = Enumerable.Empty<TaskResponse>();

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
