using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoList.DTOs;
using ToDoList.Interfaces;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTaskRequest request)
        {
            var response = await _taskService.AddAsync(request);

            return CreatedAtAction(
                nameof(Create),
                new { id = response.TaskId },
                response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] GetTasksRequest request)
        {
            var response = await _taskService.GetTasksAsync(request);

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var response = await _taskService.GetTaskByIdAsync(id);

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteTaskAsync(id);

            return NoContent();
        }
    }
}
