using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProMgt.Client.Models.Task;
using ProMgt.Components.Account;
using ProMgt.Data;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ProMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        // Db context dependency injection
        private readonly ProjectDbContext _db;
        // private readonly ILogger _logger;
        // Http Context can help in recognizing the current user
        private readonly IdentityUserAccessor _userAccessor;
        public TaskController(ProjectDbContext db, IdentityUserAccessor userAccessor)
        {
            _db = db;
            _userAccessor = userAccessor;
        }

        #region Post
        /// <summary>
        /// This creates a task
        /// </summary>
        /// <param name="newTask"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("createtask")]
        public async Task<ActionResult<TaskResponse>> CreateTask(TaskInputModel newTask)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
                if (user == null)
                {
                    return NotFound("User not found!");
                }

                var project = await _db.Projects.FindAsync(newTask.ProjectId);

                if (project == null)
                { 
                    return NotFound(); 
                }

                ProjectTask _newTask = new ProjectTask()
                {
                    Name = newTask.Name,
                    ProjectId = project.Id,
                    DateOfCreation = DateTime.Now,
                    DeadLine = newTask.DeadLine,
                    CreatedBy = user.Id,
                    Project = project
                };

                _db.Tasks.Add(_newTask);
                await _db.SaveChangesAsync();
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };

                var jsonString = JsonSerializer.Serialize(_newTask, options);
                return CreatedAtAction(nameof(GetTask), new { id = _newTask.Id }, jsonString);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                // Log the error (uncomment ex variable name and write a log.)
                return Conflict("A project with the same name already exists.");
            }
            catch (DbUpdateException ex)
            {
                // Log the error (uncomment ex variable name and write a log.)
                return UnprocessableEntity("An error occurred while updating the database. Please check your input and try again.");
            }
            catch (InvalidOperationException iox)
            {
                return Unauthorized(iox);
            }
            catch (Exception ex)
            {
                // Log the error (uncomment ex variable name and write a log.)
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        } 
        #endregion

        #region Get
        /// <summary>
        /// This gets a task by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("gettask/{id}")]
        public async Task<ActionResult<ProjectTask>> GetTask(int id)
        {
            var task = await _db.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }

            return task;

        }

        /// <summary>
        /// This gets all the task of a project
        /// </summary>
        /// <param name="projectId">Pass the id of the project to get all the tasks.</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("projecttask/{projectId}")]
        public async Task<ActionResult<List<TaskResponse>>> GetTaskByProject(int projectId)
        {
            var tasks = await _db.Tasks.Where(t => t.ProjectId == projectId).ToListAsync();
            if (tasks == null || !tasks.Any())
            {
                return NotFound();
            }

            return Ok(ConvertToTaskResponse(tasks));
        }

        #endregion

        #region Delete
        /// <summary>
        /// This deletes a task.
        /// </summary>
        /// <param name="id">Pass the id of task to delete.</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _db.Tasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                _db.Tasks.Remove(task);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the task" });
            }

        } 
        #endregion

        #region Patch

        /// <summary>
        /// This Updates the task status.
        /// </summary>
        /// <param name="id">Pass id of the task.</param>
        /// <param name="status">Pass the new status.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> PatchStatus(int id, [FromBody] bool status)
        {
            try
            {
                var task = await _db.Tasks.FindAsync(id);
                if (task == null)
                {

                    return NotFound(new { message = "Task not found" });
                }

                task.IsCompleted = status;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An error occurred while updating the task status." });
            }

        }

        /// <summary>
        /// This updates the name.
        /// </summary>
        /// <param name="id">Pass the id of the task.</param>
        /// <param name="name">Pass the new name.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/name")]
        public async Task<ActionResult> PatchName(int id, [FromBody] string name)
        {
            try
            {
                var task = await _db.Tasks.FindAsync(id);
                if (task == null)
                {

                    return NotFound(new { message = "Task not found" });
                }

                task.Name = name;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An error occurred while updating the task name" });
            }

        }

        /// <summary>
        /// This updates the task description.
        /// </summary>
        /// <param name="id">Pass id of the task.</param>
        /// <param name="description">Pass the new description.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/description")]
        public async Task<ActionResult> PatchDescription(int id, [FromBody] string description)
        {
            try
            {
                var task = await _db.Tasks.FindAsync(id);
                if (task == null)
                {

                    return NotFound(new { message = "Task not found" });
                }

                task.Description = description;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An error occurred while updating the task description" });
            }

        }

        /// <summary>
        /// This updates the deadline
        /// </summary>
        /// <param name="id">Pass id of the task.</param>
        /// <param name="deadline">Pass the new deadline.</param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/deadline")]
        public async Task<ActionResult> PatchDeadLine(int id, [FromBody] DateTime deadline)
        {
            try
            {
                var task = await _db.Tasks.FindAsync(id);
                if (task == null)
                {

                    return NotFound(new { message = "Task not found" });
                }

                task.DeadLine = deadline;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An error occurred while updating the task deadline." });
            }

        } 
        #endregion

        #region Helper converter
        /// <summary>
        /// This converts a List<ProjectTask> to a List<TaskResponse>
        /// </summary>
        /// <param name="tasks">Pass a List<ProjectTask></param>
        /// <returns></returns>
        internal List<TaskResponse> ConvertToTaskResponse(List<ProjectTask> tasks)
        {
            List<TaskResponse> localList = new List<TaskResponse>();
            foreach (var task in tasks)
            {
                localList.Add(new()
                {
                    CreatedBy = task.CreatedBy,
                    DateOfCreation = task.DateOfCreation,
                    DeadLine = task.DeadLine,
                    Description = task.Description,
                    Id = task.Id,
                    IsCompleted = task.IsCompleted,
                    Name = task.Name,
                    ProjectId = task.ProjectId
                });
            }
            return localList;
        }
        #endregion

    }
}
