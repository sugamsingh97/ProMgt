using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProMgt.Client.Models.Assignments;
using ProMgt.Client.Models.Project;
using ProMgt.Client.Models.Task;
using ProMgt.Client.Models.User;
using ProMgt.Components.Account;
using ProMgt.Data;
using ProMgt.Data.Model;
using System.Linq;

namespace ProMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskAssignmentController : ControllerBase
    {
        #region Private fields
        // Db context dependency injection
        private readonly ProjectDbContext _db;
        // private readonly ILogger _logger;
        // Http Context can help in recognizing the current user
        private readonly IdentityUserAccessor _userAccessor;

        //This is just an example. detelet this after POC
        private readonly ApplicationDbContext _applicationDbContext;
        #endregion

        public TaskAssignmentController(ProjectDbContext db, IdentityUserAccessor userAccessor, ApplicationDbContext applicationDbContext)
        {
            _db = db;
            _userAccessor = userAccessor;
            _applicationDbContext = applicationDbContext;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProjectAssignedDisplayModel>> CreateTaskAssignment(TaskAssignmentInputModel taskAssignment)
        {
            try
            {
                var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
                if (user == null)
                {
                    return NotFound("User not found!");
                }

                // checking if the asignee exists
                var assignee = await _applicationDbContext.Users.FindAsync(taskAssignment.AssigneeId);
                if (assignee == null)
                {
                    return NotFound("Assignee not found!");
                }

                // check if there is already a ProjectAssignment with User and Asignee
                var assignmentExists = await _db.TasksAssignments
                    .Where(ta => ta.UserId == user.Id
                    && ta.TaskId == taskAssignment.TaskId
                    && ta.AssigneeId == taskAssignment.AssigneeId).ToListAsync();

                if (assignmentExists.Count > 0)
                {
                    return NotFound("Member already added!");
                }

                //checking if the assignment already exist with the taskId
                var assignmentExistTaskId = await _db.TasksAssignments
                    .Where(t => t.TaskId == taskAssignment.TaskId).ToListAsync();
                  

                // check if the old asignnee if not equals to the new asignee id then, delete old one
                foreach (var item in assignmentExistTaskId)
                {
                    if (item.AssigneeId != taskAssignment.AssigneeId )
                    {
                        _db.TasksAssignments.Remove(item);
                        await _db.SaveChangesAsync();
                    }
                }

                TaskAssignment newTaskAssignment = new TaskAssignment() 
                {
                    TaskId = taskAssignment.TaskId,
                    AssigneeId = taskAssignment.AssigneeId,
                    UserId = user.Id,
                };                

                _db.TasksAssignments.Add(newTaskAssignment);
                await _db.SaveChangesAsync();
                var response = new
                {
                    Status = "Success",
                    Message = "Task assignment created successfully.",
                    Data = newTaskAssignment,
                    StatusCode = StatusCodes.Status201Created
                };

                return Created($"/api/taskassignment/{newTaskAssignment.Id}", response);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                // Log the error (uncomment ex variable name and write a log.)
                return Conflict("UNIQUE constraint failed");
            }
            catch (DbUpdateException ex)
            {
                // Log the error (uncomment ex variable name and write a log.)
                return UnprocessableEntity("An error occurred while updating the database. Please check your input and try again." + ex.Message);
            }
            catch (InvalidOperationException iox)
            {
                return Unauthorized(iox.Message);
            }
            catch (Exception ex)
            {
                // Log the error (uncomment ex variable name and write a log.)
                return StatusCode(500, "An unexpected error occurred. Please try again later." + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskAssignmentDisplayModel>> GetTaskAssignment(int id)
        {
            var taskAssignment = await _db.TasksAssignments.FindAsync(id);

            if (taskAssignment == null)
            {
                return NotFound();
            }

            TaskAssignmentDisplayModel _taskAssignment = new TaskAssignmentDisplayModel()
            {
                UserId = taskAssignment.UserId,
                TaskId = taskAssignment.TaskId,
                AsigneeId = taskAssignment.AssigneeId
            };
            return _taskAssignment;
        }

        // find the Taskassignment witht the task Id and then fend back the user associatd to the Task.
        [HttpGet("getassignee/{id}")]
        public async Task<ActionResult<UserResponse>> GetTaskAssignee(int id)
        {
            UserResponse taskAssignee = new UserResponse();

            var response = await _db.TasksAssignments
                .Where(ta => ta.TaskId == id)
                .FirstOrDefaultAsync();

            if (response == null)
            {
                return NotFound("No assignment found for the given task ID.");
            }

            var user = await _applicationDbContext.Users
                .Where(u => u.Id == response.AssigneeId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("No user found for the given assignment.");
            }

            taskAssignee.UserId = user.Id;
            taskAssignee.Email = user.Email;
            taskAssignee.FirstName = user.FirstName;
            taskAssignee.LastName = user.LastName;

            return Ok(taskAssignee);
        }

        // this deletes the project asignment
        [Authorize]
        [HttpDelete("asignee/{_asigneeId}")]
        public async Task<ActionResult> DeleteTaskAssignment(string _asigneeId)
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            try
            {
                var taskassignment = await _db.TasksAssignments.Where(ta => ta.AssigneeId == _asigneeId).FirstOrDefaultAsync();
               
                if (taskassignment == null)
                {
                    return NotFound(new { message = "Taskassignment not found" });
                }

                if (user.Id != taskassignment.UserId)
                {
                    return NotFound(new { message = "Only project owner can delete task assignment." });
                }

                _db.TasksAssignments.Remove(taskassignment);                
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the task assignment" + ex.Message });
            }

        }
    }
}
