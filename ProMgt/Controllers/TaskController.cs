using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProMgt.Client.Models.Task;
using ProMgt.Components.Account;
using ProMgt.Data;
using System.Text.Json.Serialization;
using System.Text.Json;
using ProMgt.Data.Model;

namespace ProMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ProjectDbContext _db;
        private readonly IdentityUserAccessor _userAccessor;

        public TaskController(ProjectDbContext db, IdentityUserAccessor userAccessor)
        {
            _db = db;
            _userAccessor = userAccessor;
        }

        #region POST
        /// <summary>
        /// This creates new task
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
                    return NotFound("Project not found!");
                }

                ProjectTask _newTask = new ProjectTask()
                {
                    Name = newTask.Name,
                    ProjectId = project.Id,
                    DateOfCreation = DateTime.Now,
                    DeadLine = newTask.DeadLine,
                    CreatedBy = user.Id,
                    Project = project,
                    SectionId = newTask.SectionId,
                };

                _db.ProjectTasks.Add(_newTask);
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
                return Conflict("A task with the same name already exists.");
            }
            catch (DbUpdateException)
            {
                return UnprocessableEntity("An error occurred while updating the database. Please check your input and try again.");
            }
            catch (InvalidOperationException iox)
            {
                return Unauthorized(iox.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }
        #endregion

        #region GET
        /// <summary>
        /// This gets a task by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("gettask/{id}")]
        public async Task<ActionResult<ProjectTask>> GetTask(int id)
        {
            try
            {
                var task = await _db.ProjectTasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound("Task not found.");
                }
                return task;
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving the task.");
            }
        }

        [Authorize]
        [HttpGet("projecttask/{projectId}")]
        public async Task<ActionResult<List<TaskResponse>>> GetTaskByProject(int projectId)
        {
            try
            {
                var tasks = await _db.ProjectTasks
                    .Where(t => t.ProjectId == projectId)
                    .Include(t => t.Project)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskStatus.Color)
                    .Include(t => t.Priority)
                    .Include(t => t.Priority.Color)
                    .Include(t => t.Section)
                    .ToListAsync();

                if (tasks == null || !tasks.Any())
                {
                    return NotFound($"No tasks found for project with ID {projectId}.");
                }

                return Ok(ConvertToTaskResponse(tasks));
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving tasks for the project.");
            }
        }

        //Get all the assigned task
        [Authorize]
        [HttpGet("assignedtask/{projectId}")]
        public async Task<ActionResult<List<TaskResponse>>> GetAssignedTask(int projectId)
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            try
            {
                // tasks by project Id
                var projectTasks = await _db.ProjectTasks
                    .Where(t => t.ProjectId == projectId)
                    .Include(t => t.Project)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskStatus.Color)
                    .Include(t => t.Priority)
                    .Include(t => t.Priority.Color)
                    .Include(t => t.Section)
                    .ToListAsync();

                // tasks assignment by logged in user
                var taskAssignments = await _db.TasksAssignments
                    .Where(t => t.AssigneeId == user.Id).ToListAsync();

                // new list of tasks which is gonna be populated with only the tasks assigned to 
                List<ProjectTask> tasks = new();

                // for each task assignment we are finding the task in the projectTasks list and adding it to the tasks list.
                foreach (var ts in taskAssignments)
                {
                    foreach (var task in projectTasks)
                    {
                        if (ts.TaskId == task.Id)
                        {
                            tasks.Add(task);
                        }
                    }
                }

                if (tasks == null || !tasks.Any())
                {
                    return NotFound($"No tasks found for project with ID {projectId}.");
                }

                return Ok(ConvertToTaskResponse(tasks));
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving tasks for the project.");
            }
        }

        [Authorize]
        [HttpGet("tasksection/{sectionId}")]
        public async Task<ActionResult<List<TaskResponse>>> GetTaskBySection(int sectionId)
        {
            try
            {
                var tasks = await _db.ProjectTasks
                    .Where(t => t.SectionId == sectionId)
                    .Include(t => t.Project)
                    .Include(t => t.TaskStatus)
                    .Include(t => t.TaskStatus.Color)
                    .Include(t => t.Priority)
                    .Include(t => t.Priority.Color)
                    .Include(t => t.Section)
                    .ToListAsync();

                if (tasks == null || !tasks.Any())
                {
                    return NotFound($"No tasks found for project with ID {sectionId}.");
                }

                return Ok(ConvertToTaskResponse(tasks));
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving tasks by section Id.");
            }
        }
        #endregion

        #region DELETE
        /// <summary>
        /// This deletes a task by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id)
        {
            try
            {
                var task = await _db.ProjectTasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                _db.ProjectTasks.Remove(task);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the task" });
            }
        }
        #endregion

        #region PATCH
        /// <summary>
        /// This Updates the status of a task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> PatchStatus(int id, [FromBody] bool status)
        {
            // only allow either the creater of the task or the asignee of the task
            try
            {
                var task = await _db.ProjectTasks.FindAsync(id);
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
        /// This updates the name of the task.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/name")]
        public async Task<ActionResult> PatchName(int id, [FromBody] string name)
        {
            try
            {
                var task = await _db.ProjectTasks.FindAsync(id);
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
        /// This update the priority Id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="_priorityId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/priority")]
        public async Task<ActionResult> PatchPriority(int id, [FromBody] int _priorityId)
        {
            try
            {
                var task = await _db.ProjectTasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }
                
                task.PriorityId = _priorityId;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task priority" });
            }
        }

        /// <summary>
        /// This updates the task status.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="_taskStatusId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/taskstatus")]
        public async Task<ActionResult> PatchTaskStatus(int id, [FromBody] int _taskStatusId)
        {
            try
            {
                var task = await _db.ProjectTasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                task.TaskStatusId = _taskStatusId;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task priority" });
            }
        }

        [Authorize]
        [HttpPatch("{id}/section")]
        public async Task<ActionResult> PatchSection(int id, [FromBody] int _sectionId)
        {
            try
            {
                var task = await _db.ProjectTasks.FindAsync(id);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                task.SectionId = _sectionId;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the task priority" });
            }
        }

        /// <summary>
        /// This updates the description.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/description")]
        public async Task<ActionResult> PatchDescription(int id, [FromBody] string description)
        {
            try
            {
                var task = await _db.ProjectTasks.FindAsync(id);
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
        /// This updates the date.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deadline"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/deadline")]
        public async Task<ActionResult> PatchDeadLine(int id, [FromBody] DateTime deadline)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var task = await _db.ProjectTasks.FindAsync(id);
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
        internal List<TaskResponse> ConvertToTaskResponse(List<ProjectTask> tasks)
        {
            List<TaskResponse> localList = new List<TaskResponse>();
            foreach (var task in tasks)
            {
                localList.Add(new()
                {
                    Id = task.Id,
                    Name = task.Name,
                    ProjectId = task.Project.Id,
                    Description = task.Description,

                    DateOfCreation = task.DateOfCreation,
                    DeadLine = task.DeadLine,
                    CreatedBy = task.CreatedBy,
                    IsCompleted = task.IsCompleted,

                    PriorityId = task.Priority?.Id ?? 0,
                    PriorityName = task.Priority?.Name ?? string.Empty,
                    PriorityHexcode = task.Priority?.Color?.HexCode ?? string.Empty,


                    TaskStatusId = task.TaskStatus?.Id ?? 0,
                    TaskStatusName = task.TaskStatus?.Name ?? string.Empty,
                    TaskStatusHexcode = task.TaskStatus?.Color?.HexCode ?? string.Empty,

                    SectionId = task.Section?.Id ?? 0,
                });
            }
            return localList;
        }
        #endregion
    }
}
