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
    public class ProjectAssignmentController : ControllerBase
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

        public ProjectAssignmentController(ProjectDbContext db, IdentityUserAccessor userAccessor, ApplicationDbContext applicationDbContext)
        {
            _db = db;
            _userAccessor = userAccessor;
            _applicationDbContext = applicationDbContext;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ProjectAssignedDisplayModel>> CreateAssignment(ProjectAssignedInputModel projectAssignment)
        {
            try
            {
                var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
                if (user == null)
                {
                    return NotFound("User not found!");
                }

                var assignee = await _applicationDbContext.Users.FindAsync(projectAssignment.AssigneeId);
                if (assignee == null)
                {
                    return NotFound("Assignee not found!");
                }
                // check if there is already a ProjectAssignment with User and Asignee
                var assignmentExists = await _db.ProjectAssignments
                    .Where(pa => pa.UserId == user.Id 
                    && pa.ProjectId == projectAssignment.ProjectId 
                    && pa.AssigneeId == assignee.Id).ToListAsync();

                if (assignmentExists.Count > 0)
                {
                    return NotFound("Member already added!");
                }

                ProjectAssignment newProjectAssignment = new ProjectAssignment()
                {
                    ProjectId = projectAssignment.ProjectId,
                    AssigneeId = assignee.Id,
                    UserId = user.Id
                };

                _db.ProjectAssignments.Add(newProjectAssignment);
                await _db.SaveChangesAsync();
                var response = new
                {
                    Status = "Success",
                    Message = "Project assignment created successfully",
                    Data = newProjectAssignment,
                    StatusCode = StatusCodes.Status201Created
                };

                return Created($"/api/projectassignment/{newProjectAssignment.Id}", response);
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
        public async Task<ActionResult<ProjectAssignedDisplayModel>> GetProjectAssignment(int id)
        {
            var projectAssignment = await _db.ProjectAssignments.FindAsync(id);

            if (projectAssignment == null)
            {
                return NotFound();
            }

            ProjectAssignedDisplayModel _projectAssignment = new ProjectAssignedDisplayModel() 
            {
                UserId = projectAssignment.UserId,
                ProjectId = projectAssignment.ProjectId,
                AsigneeId = projectAssignment.AssigneeId
            };
           
          


            return _projectAssignment;
        }

        [Authorize]
        [HttpGet("getmembers/{id}")]
        public async Task<ActionResult<List<UserResponse>>> GetProjectMembers(int id)
        {            
            // get list of all the assignments of the project
            var lisOfProjectAssignments = await _db.ProjectAssignments.Where(pa => pa.ProjectId == id).ToListAsync();

            // and then for each projectassignment take the string id of assignee and look for themm and add it to a userResponse list.
            List<UserResponse> members = new List<UserResponse>();
            if (lisOfProjectAssignments.Count<1)
            {
                return members;
            }
            foreach (var asignee in lisOfProjectAssignments)
            {              
                var _assignedUser = await _applicationDbContext.Users.FindAsync(asignee.AssigneeId);
                UserResponse asignedUser = new() { 
                    UserId = _assignedUser.Id,
                    FirstName = _assignedUser.FirstName,
                    LastName = _assignedUser.LastName,
                    Email = _assignedUser.Email                    
                };
                members.Add(asignedUser);
            }

            return members;
           
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAssignment(int Id)
        {
            try
            {
                var projectAssignment = await _db.ProjectAssignments.FindAsync(Id);
                if (projectAssignment == null)
                {
                    return NotFound(new { message = "Project assignment not found" });
                }

                _db.ProjectAssignments.Remove(projectAssignment);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the assignment" + ex.Message });
            }

        }
    }
}
