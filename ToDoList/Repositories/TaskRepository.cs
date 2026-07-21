using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.DTOs;
using ToDoList.Entities;
using ToDoList.Interfaces;
using ToDoList.Models;

namespace ToDoList.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem> AddAsync(TaskItem task)
        {
            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<PagedTaskResult> GetTasksAsync(GetTasksRequest request)
        {
            var query = _context.Tasks
                        .AsNoTracking()
                        .AsQueryable();

            var totalCount = await query.CountAsync();

            var tasks = await query
                .Include(t => t.Status)
                .OrderBy(t => t.DueDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedTaskResult
            {
                Items = tasks,
                TotalCount = totalCount
            };
        }
    }

}
