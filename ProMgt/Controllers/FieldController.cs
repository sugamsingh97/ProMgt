using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProMgt.Client.Models.Task;
using ProMgt.Components.Account;
using ProMgt.Data;
using System.Text.Json.Serialization;
using System.Text.Json;
using ProMgt.Client.Models.Fields.Priority;
using ProMgt.Client.Models.Fields.TaskStatus;
using TaskStatus = ProMgt.Data.Model.TaskStatus;
using ProMgt.Data.Model;

namespace ProMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly ProjectDbContext _db;
        private readonly IdentityUserAccessor _userAccessor;

        public FieldController(ProjectDbContext db, IdentityUserAccessor userAccessor)
        {
            _db = db;
            _userAccessor = userAccessor;
        }

        #region Post Priority
        [Authorize]
        [HttpPost("createpriority")]
        public async Task<ActionResult<PriorityResponse>> CreatePriority(PriorityInputModel newPriority)
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

                var project = await _db.Projects.FindAsync(newPriority.projectId);
                if (project == null)
                {
                    return NotFound("Project not found!");
                }

                Priority _newPriority = new Priority()
                {
                    Name = newPriority.Name,
                    ProjectId = project.Id,
                    ColorId = newPriority.ColorId
                };

                _db.Priorities.Add(_newPriority);
                await _db.SaveChangesAsync();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                var jsonString = JsonSerializer.Serialize(_newPriority, options);

                return CreatedAtAction(nameof(GetPriority), new { id = _newPriority.Id }, jsonString);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                return Conflict("A project with the same name already exists.");
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

        #region Get Priority by Priority Id
        [Authorize]
        [HttpGet("getpriority/{id}")]
        public async Task<ActionResult<PriorityResponse>> GetPriority(int id)
        {
            try
            {
                var priority = await _db.Priorities.Include(p => p.Color).FirstOrDefaultAsync(p => p.Id == id);
                if (priority == null)
                {
                    return NotFound("Priority not found.");
                }

                PriorityResponse _priority = new()
                {
                    Id = priority.Id,
                    Name = priority.Name,
                    ColorId = priority.ColorId,
                    projectId = priority.ProjectId,
                    HexColor = priority.Color?.HexCode
                };

                return _priority;
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the priority.");
            }
        }
        #endregion

        #region Get all Priorities by projectId
        [Authorize]
        [HttpGet("projectpriority/{projectId}")]
        public async Task<ActionResult<List<PriorityResponse>>> GetPriorityByProject(int projectId)
        {
            try
            {


                //var priorities = await _db.Priorities
                //    .Include(x => x.Color)
                //    .Where(p => p.projectId == projectId)
                //    .ToListAsync();

                var priorities = await _db.Priorities.Include(x => x.Color).Where(p => p.ProjectId == projectId).ToListAsync();

                //if (priorities == null || !priorities.Any())
                //{
                //    return NotFound($"No priorities found for project with ID {projectId}.");
                //}

                List<PriorityResponse> _priorities = priorities.Select(priority => new PriorityResponse
                {
                    Id = priority.Id,
                    Name = priority.Name,
                    ColorId = priority.ColorId,
                    projectId = priority.ProjectId,
                    HexColor = priority.Color?.HexCode
                }).ToList();

                return Ok(_priorities);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }
        #endregion

        #region Delete Priority
        [Authorize]
        [HttpDelete("deletepriority{Id}")]
        public async Task<ActionResult> DeletePriority(int Id)
        {
            try
            {
                var priority = await _db.Priorities.FindAsync(Id);
                if (priority == null)
                {
                    return NotFound(new { message = "Priority not found" });
                }

                _db.Priorities.Remove(priority);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the priority" });
            }
        }
        #endregion

        #region Post Task Status
        [Authorize]
        [HttpPost("createtaskstatus")]
        public async Task<ActionResult<TaskStatusResponse>> CreateTaskStatus(TaskStatusInputModel newtaskStatus)
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

                var project = await _db.Projects.FindAsync(newtaskStatus.ProjectId);
                if (project == null)
                {
                    return NotFound("Project not found!");
                }

                TaskStatus _newTaskStatus = new TaskStatus()
                {
                    Name = newtaskStatus.Name,
                    ProjectId = newtaskStatus.ProjectId,
                    ColorId = newtaskStatus.ColorId
                };

                _db.TaskStatuses.Add(_newTaskStatus);
                await _db.SaveChangesAsync();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                var jsonString = JsonSerializer.Serialize(_newTaskStatus, options);

                return CreatedAtAction(nameof(GetTaskStatus), new { id = _newTaskStatus.Id }, jsonString);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                return Conflict("A task status with the same name already exists.");
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

        #region Get Task Status by Task status Id
        [Authorize]
        [HttpGet("gettaskstatus/{id}")]
        public async Task<ActionResult<TaskStatusResponse>> GetTaskStatus(int id)
        {
            try
            {
                var taskStatus = await _db.TaskStatuses.Include(ts => ts.Color).FirstOrDefaultAsync(ts => ts.Id == id);
                //if (taskStatus == null)
                //{
                //    return NotFound("Task status not found.");
                //}

                TaskStatusResponse _taskStatus = new()
                {
                    Id = taskStatus.Id,
                    Name = taskStatus.Name,
                    ColorId = taskStatus.ColorId,
                    ProjectId = taskStatus.ProjectId,
                    HexColor = taskStatus.Color?.HexCode
                };

                return Ok(_taskStatus);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the task status.");
            }
        }
        #endregion

        #region Get all Task Statuses by Project Id
        [Authorize]
        [HttpGet("projecttaskstatus/{projectId}")]
        public async Task<ActionResult<List<TaskStatusResponse>>> GetTaskStatusByProject(int projectId)
        {
            try
            {
                var taskStatuses = await _db.TaskStatuses
                    .Include(x => x.Color)
                    .Where(p => p.ProjectId == projectId)
                    .ToListAsync();

                if (taskStatuses == null || !taskStatuses.Any())
                {
                    return NotFound($"No task statuses found for project with ID {projectId}.");
                }

                List<TaskStatusResponse> _taskStatuses = taskStatuses.Select(ts => new TaskStatusResponse
                {
                    Id = ts.Id,
                    Name = ts.Name,
                    ColorId = ts.ColorId,
                    ProjectId = ts.ProjectId,
                    HexColor = ts.Color?.HexCode
                }).ToList();

                return Ok(_taskStatuses);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }
        #endregion

        #region Delete Task Status
        [Authorize]
        [HttpDelete("deletetaskstatus{Id}")]
        public async Task<ActionResult> DeleteTaskStatus(int Id)
        {
            try
            {
                var taskStatus = await _db.TaskStatuses.FindAsync(Id);
                if (taskStatus == null)
                {
                    return NotFound(new { message = "Task status not found" });
                }

                _db.TaskStatuses.Remove(taskStatus);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the task status" });
            }
        }
        #endregion
    }
}
