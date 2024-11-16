using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProMgt.Client.Models.Project;
using ProMgt.Client.Models.Task;
using ProMgt.Client.Models.User;
using ProMgt.Components.Account;
using ProMgt.Data;
using ProMgt.Data.Model;
using static ProMgt.Client.Components.Dialogs.CreateProjectDialog;

namespace ProMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
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

        #region Constructor
        public ProjectController(ProjectDbContext db, IdentityUserAccessor userAccessor, ApplicationDbContext applicationDbContext)
        {
            _db = db;
            // _logger = logger;
            _userAccessor = userAccessor;

            _applicationDbContext = applicationDbContext;
        }
        #endregion

        //
       
        [HttpGet("getAppUserlist")]
        public async Task<ActionResult<List<UserResponse>>> GetUserList()
        {
            var users = await _applicationDbContext.Users.ToListAsync() ;
            var _users = users.Select(user => new UserResponse { 
                UserId = user.Id, 
                FirstName = user.FirstName, 
                LastName = user.LastName, 
                Email = user?.Email }).ToList(); 
            return _users;
           
        }

        #region POST
        /// <summary>
        /// This Action method creates the project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(ProjectCreateModel project)
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

                Project tempProject = new()
                {
                    Name = project.Name,
                    DeadLine = project.DeadLine,
                    DateOfCreation = DateTime.Now,
                    CreatedBy = user.Id.ToString()
                };

                _db.Projects.Add(tempProject);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProject), new { id = tempProject.Id }, tempProject);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                // Log the error (uncomment ex variable name and write a log.)
                return Conflict("A project with the same name already exists.");
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
        #endregion

        #region GET
        /// <summary>
        /// This Action method gets the list of all projects
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [HttpGet("getprojectlist")]
        public async Task<ActionResult<List<ProjectDisplay>>> GetProjects()
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);

            var projects = await _db.Projects.Where(p => p.CreatedBy == user.Id).Include(p => p.Tasks).ToListAsync();
            var projectResponse = projects.Select( project => new ProjectDisplay {
                Id = project.Id,
                Name = project.Name,
                DateOfCreation = project.DateOfCreation,
                DeadLine = project.DeadLine,
                IsCompleted = project.IsCompleted,
                ProjectStatusId = project.ProjectStatusId
            }).ToList();
            return Ok(projectResponse);
        }

        [HttpGet("getprojectswithtasks")]
        public async Task<ActionResult<List<ProjectResponse>>> GetProjectsAndTask()
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            var projects = await _db.Projects
                .Where(p => p.CreatedBy == user.Id)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Priority)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.TaskStatus)
                .ToListAsync();

            var projectResponses = projects.Select(project => new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                ProjectSummery = project.ProjectSummery,
                DateOfCreation = project.DateOfCreation,
                DeadLine = project.DeadLine,
                CreatedBy = project.CreatedBy,
                IsCompleted = project.IsCompleted,
                ProjectStatusId = project.ProjectStatusId,
                Tasks = project.Tasks?.Select(task => new TaskResponse
                {
                    Id = task.Id,
                    Name = task.Name,
                    ProjectId = task.ProjectId,
                    Description = task.Description,
                    TaskSummery = task.ProjectSummery,
                    DateOfCreation = task.DateOfCreation,
                    DeadLine = task.DeadLine,
                    CreatedBy = task.CreatedBy,
                    IsCompleted = task.IsCompleted,
                    PriorityId = task.PriorityId,
                    TaskStatusId = task.TaskStatusId,
                    SectionId = task.SectionId ?? 0,
                    PriorityName = task.Priority?.Name,
                    TaskStatusName = task.TaskStatus?.Name                    
                }).ToList() ?? new List<TaskResponse>()
            }).ToList();

            return Ok(projectResponses);
        }

        /// <summary>
        /// This Action methods gets a project by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponse>> GetProject(int id)
        {
            var project = await _db.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }          

            ProjectResponse projectResponse = new ProjectResponse();
            projectResponse.Id = project.Id;
            projectResponse.Name = project.Name;
            projectResponse.Description = project.Description;
            projectResponse.DateOfCreation = project.DateOfCreation;
            projectResponse.DeadLine = project.DeadLine;
            projectResponse.CreatedBy = project.CreatedBy;
            projectResponse.IsCompleted = project.IsCompleted;
            projectResponse.ProjectStatusId = project.ProjectStatusId;


            return projectResponse;
        }

        /// <summary>
        /// This get all the project statuses
        /// </summary>
        /// <returns></returns>
        [HttpGet("getprojectstatuses")]
        public async Task<ActionResult<List<ProjectStatus>>> GetProjectStatuses()
        {
            var projectStatuses = await _db.ProjectStatuses.ToListAsync();
            return Ok(projectStatuses);
        }       

        [HttpGet("getcolors")]
        public async Task<ActionResult<List<ProjectColorResponse>>> GetColors()
        {
            var projectColors = await _db.ProjectMgtColors.ToListAsync();
            var projectColorResponses = projectColors.Select(pc => new ProjectColorResponse
            {
                Id = pc.Id,
                Name = pc.Name,
                HexCode = pc.HexCode
            }).ToList();

            return Ok(projectColorResponses);
        } 

        /// <summary>
        /// This get the hex code of the Color
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("gethexcode/{id}")]

        public async Task<ActionResult<string>> GetHexCode(int id)
        {

            var _color = await _db.ProjectMgtColors.FindAsync(id);

            if (_color == null)
            {
                return NotFound();
            }

            string? _hexCode = _color.HexCode;


            var result = new { color = _hexCode };

            return Ok(result);
        }

        //Get projects by asignees
        [Authorize]
        [HttpGet]
        [HttpGet("getassignedprojectlist")]
        public async Task<ActionResult<List<ProjectDisplay>>> GetAssignedProjects()
        {
            List<ProjectResponse> projects = new();
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);

            // we are geting the project assignments assigned to a user
            var projectAssignments = await _db.ProjectAssignments.Where(pa => pa.AssigneeId == user.Id).ToListAsync();
            // for each assignedmenet we are collecting the project and adding it to the project response list. 
            foreach (var pa in projectAssignments)
            {
                var response = await _db.Projects.FindAsync(pa.ProjectId);
                ProjectResponse _project = new ProjectResponse() 
                {
                    Name = response.Name,
                    DeadLine = response.DeadLine,
                    CreatedBy = response.CreatedBy,
                    DateOfCreation = response.DateOfCreation,
                    Description = response.Description,
                    Id = response.Id,
                    IsCompleted = response.IsCompleted,
                    ProjectStatusId = response.ProjectStatusId,  
                };
                projects.Add(_project);
            }
            return Ok(projects);
        }

        #endregion        

        #region PUT
        /// <summary>
        /// This Updates the whole project record
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        public async Task<ActionResult<Project>> UpdateProject(ProjectInputModel project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                //find the record
                var _project = await _db.Projects.FindAsync(project.Id);

                //record dont exist throw error
                if (_project == null)
                {
                    return NotFound("This record does not exist");
                }
                //record exist assign new value to the record
                _project.Name = project.Name;
                _project.Description = project.Description;
                _project.DeadLine = project.DeadLine;

                _db.Projects.Update(_project);
                await _db.SaveChangesAsync();

                return Ok(_project);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Patch
        /// <summary>
        /// This updates name
        /// </summary>
        /// <param name="id">Pass the project id</param>
        /// <param name="name">Pass the project name</param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/name")]
        public async Task<ActionResult> PatchName(int id, [FromBody] string name)
        {
            try
            {
                var project = await _db.Projects.FindAsync(id);
                if (project == null)
                {

                    return NotFound(new { message = "Project not found" });
                }

                project.Name = name;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An error occurred while updating the project name" });
            }
           
        }

        /// <summary>
        /// This updates project status.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/statusid")]
        public async Task<ActionResult> PatchStatus(int id, [FromBody] int statusId)
        {
            try
            {
                var project = await _db.Projects.FindAsync(id);
                if (project == null)
                {

                    return NotFound(new { message = "Project not found" });
                }

                project.ProjectStatusId = statusId;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An error occurred while updating the project name" });
            }

        }

        /// <summary>
        /// This updates description
        /// </summary>
        /// <param name="id">Pass the project id</param>
        /// <param name="description">Pass the description</param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/description")]
        public async Task<ActionResult> PatchDescription(int id, [FromBody] string description)
        {
            try
            {
                var project = await _db.Projects.FindAsync(id);
                if (project == null)
                {

                    return NotFound(new { message = "Project not found" });
                }

                project.Description = description;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An error occurred while updating the project name" });
            }

        }

        /// <summary>
        /// This updates the dead line
        /// </summary>
        /// <param name="id">Pass the project id</param>
        /// <param name="deadline">Pass the new deadline</param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch("{id}/deadline")]
        public async Task<ActionResult> PatchDeadLine(int id, [FromBody] DateTime deadline)
        {
            try
            {
                var project = await _db.Projects.FindAsync(id);
                if (project == null)
                {

                    return NotFound(new { message = "Project not found" });
                }

                project.DeadLine = deadline;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An error occurred while updating the project name" });
            }

        }

        [Authorize]
        [HttpPatch("{id}/iscompleted")]
        public async Task<ActionResult> PatchCompletion(int id, [FromBody] bool isCompleted)
        {
            try
            {
                var project = await _db.Projects.FindAsync(id);
                if (project == null)
                {

                    return NotFound(new { message = "Project not found" });
                }

                project.IsCompleted = isCompleted;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {

                return StatusCode(500, new { message = "An error occurred while updating the project name" });
            }

        }

        #endregion

        #region Delete
        /// <summary>
        /// This deletes the project
        /// </summary>
        /// <param name="Id">Pass the project Id</param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteProject(int Id)
        {
            try
            {
                var project = await _db.Projects.FindAsync(Id);
                if (project == null)
                {
                    return NotFound(new { message = "Project not found" });
                }

                _db.Projects.Remove(project);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the project" + ex.Message });
            }          

        } 
        #endregion

    }
}
