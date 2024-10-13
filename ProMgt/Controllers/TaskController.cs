﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly ProjectDbContext _db;
        private readonly IdentityUserAccessor _userAccessor;

        public TaskController(ProjectDbContext db, IdentityUserAccessor userAccessor)
        {
            _db = db;
            _userAccessor = userAccessor;
        }

        #region Post
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

        #region Get
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
        #endregion

        #region Delete
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

        #region Patch
        [Authorize]
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> PatchStatus(int id, [FromBody] bool status)
        {
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
                    CreatedBy = task.CreatedBy,
                    DateOfCreation = task.DateOfCreation,
                    DeadLine = task.DeadLine,
                    ProjectId = task.Project.Id,
                    PriorityId = task.Priority?.Id ?? 0,
                    Description = task.Description,
                    IsCompleted = task.IsCompleted,
                    TaskStatusId = task.TaskStatus?.Id ?? 0,
                    PriorityHexcode = task.Priority?.Color?.HexCode,
                    TaskStatusHexcode = task.TaskStatus?.Color?.HexCode
                });
            }
            return localList;
        }
        #endregion
    }
}
