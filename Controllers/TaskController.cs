using Microsoft.AspNetCore.Mvc;
using TaskManager_Simplex_.Context;
using TaskManager_Simplex_.Models;
using TaskManager_Simplex_.Repositories.Interface;
using System.Threading.Tasks;

namespace TaskManager_Simplex_.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepo _taskRepo;
        private readonly AppDbContext _context;

        public TaskController(ITaskRepo taskRepo, AppDbContext context)
        {
            _taskRepo = taskRepo;
            _context = context;
        }

        //To Create Tasks
        [HttpPost("CreateTask")]
        public async Task<ActionResult<Tasks>> CreateTask([FromBody] Tasks task)
        {
            if (task == null)
                return BadRequest();

            // Check if the provided data is valid
            if (!ModelState.IsValid)
                return BadRequest("Invalid request data.");

            // Check if the assigned user exists
            var assignedUser = await _context.Users.FindAsync(task.AssignedUserId);
            if (assignedUser == null)
                return NotFound(new { Message = "Assigned user not found" });

            // Add task to repository
            var createdTask = await _taskRepo.CreateTaskAsync(task);

            return Ok(new
            {
                Message = "Task created",
                Task = new
                {
                    Id = createdTask.Id,
                    Title = createdTask.Title,
                    Description = createdTask.Description,
                    DueDate = createdTask.DueDate,
                    Status = createdTask.Status,
                    AssignedUserId = createdTask.AssignedUserId,
                }
            });
        }

        //To Get All Tasks
        [HttpGet("GetAllTasks")]
        public async Task<ActionResult<IEnumerable<Tasks>>> GetAllTasks()
        {
            var tasks = await _taskRepo.GetAllTasksAsync();
            return Ok(tasks);
        }

        //To Update Task
        [HttpPut("UpdateTask/{id}")]
        public async Task<ActionResult<Tasks>> UpdateTask(int id, [FromBody] Tasks task)
        {
            if (id != task.Id)
                return BadRequest("Invalid task ID");

            var existingTask = await _taskRepo.GetTaskByIdAsync(id);
            if (existingTask == null)
                return NotFound(new { Message = "Task not found" });

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.DueDate = task.DueDate;
            existingTask.Status = task.Status;
            existingTask.AssignedUserId = task.AssignedUserId;

            var updatedTask = await _taskRepo.UpdateTaskAsync(existingTask);

            return Ok(new
            {
                Message = "Task updated",
                Task = updatedTask
            });
        }

        //To Delete Tasks
        [HttpDelete("DeleteTask/{id}")]
        public async Task<ActionResult<Tasks>> DeleteTask(int id)
        {
            var taskToDelete = await _taskRepo.GetTaskByIdAsync(id);
            if (taskToDelete == null)
                return NotFound(new { Message = "Task not found" });

            var deletedTask = await _taskRepo.DeleteTaskAsync(id);

            return Ok(new
            {
                Message = "Task deleted",
                Task = deletedTask
            });
        }

    }
}


