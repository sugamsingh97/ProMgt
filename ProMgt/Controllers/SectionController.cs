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
using ProMgt.Client.Models.Section;
using ProMgt.Data.Model;

namespace ProMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SectionController : ControllerBase
    {
        private readonly ProjectDbContext _db;
        private readonly IdentityUserAccessor _userAccessor;

        public SectionController(ProjectDbContext db, IdentityUserAccessor userAccessor)
        {
            _db = db;
            _userAccessor = userAccessor;
        }

        #region POST
        /// <summary>
        /// This creates sections
        /// </summary>
        /// <param name="newSection"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("createsection")]
        public async Task<ActionResult<SectionResponse>> CreateSection(SectionInputModel newSection)
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

                var project = await _db.Projects.FindAsync(newSection.ProjectId);
                if (project == null)
                {
                    return NotFound("Project not found!");
                }

                Section _newSection = new Section()
                {
                    Name = newSection.Name,
                    ProjectId = project.Id,
                    
                };

                _db.Sections.Add(_newSection);
                await _db.SaveChangesAsync();

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    WriteIndented = true
                };
                var jsonString = JsonSerializer.Serialize(_newSection, options);

                return CreatedAtAction(nameof(GetSection), new { id = _newSection.Id }, jsonString);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                return Conflict("A Section with the same name already exists.");
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
        /// This get a single section by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("getsection/{id}")]
        public async Task<ActionResult<SectionResponse>> GetSection(int id)
        {
            try
            {
                var section = await _db.Sections.FindAsync(id);
                if (section == null)
                {
                    return NotFound("Section not found.");
                }

                SectionResponse _section = new()
                {
                    Id = section.Id,
                    Name = section.Name,
                    ProjectId = section.ProjectId,
                };

                return _section;
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the priority.");
            }
        }

   
        /// <summary>
        /// This gets all the section by Project Id.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("projectsection/{projectId}")]
        public async Task<ActionResult<List<SectionResponse>>> GetSectionByProject(int projectId)
        {
            try
            {  
                var sections = await _db.Sections.Where(s=>s.ProjectId==projectId).ToListAsync();

                if (sections == null || !sections.Any())
                {
                    return NotFound($"No sections found for project with Id {projectId}.");
                }

                List<SectionResponse> _sections = sections.Select(section => new SectionResponse
                {
                    Id = section.Id,
                    Name = section.Name,
                    ProjectId = section.ProjectId,
                }).ToList();

                return Ok(_sections);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }

        /// <summary>
        /// This gets the default section.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet("projectdefaultsection/{projectId}")]
        public async Task<ActionResult<SectionResponse>> GetDefaultSectionByProject(int projectId, int excludeSectionId)
        {
            try
            {
                var section = await _db.Sections
                    .Where(s => s.ProjectId == projectId && s.Id != excludeSectionId)
                    .OrderBy(s => s.Id) // Or use s.CreationDate if you have one
                    .FirstOrDefaultAsync();

                if (section == null)
                {
                    return NotFound($"No sections found for project with Id {projectId}.");
                }

                var sectionResponse = new SectionResponse
                {
                    Id = section.Id,
                    Name = section.Name,
                    ProjectId = section.ProjectId,
                };

                return Ok(sectionResponse);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }
        #endregion

        #region DELETE
        /// <summary>
        /// This deletes a section
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("deletesection/{Id}")]
        public async Task<ActionResult> DeleteSection(int Id)
        {
            try
            {
                var section = await _db.Sections.FindAsync(Id);
                if (section == null)
                {
                    return NotFound(new { message = "Section not found" });
                }

                int totalSections = await _db.Sections.Where(sec=>sec.ProjectId ==section.ProjectId).CountAsync();

                // Checking if there ar at least more than one Section
                // In case where there are no more than one section let, do not let user to delete the remaining one section.
                if (totalSections <= 1)
                {
                    return BadRequest(new { message = "Cannot delete the last remaining section." });
                }

                _db.Sections.Remove(section);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the priority" });
            }
        }
        #endregion

        #region PATCH
        [Authorize]
        [HttpPatch("{id}/name")]
        public async Task<ActionResult> PatchName(int id, [FromBody] string name)
        {
            try
            {
                var section = await _db.Sections.FindAsync(id);
                if (section == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                section.Name = name;
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating the section name" });
            }
        }
        #endregion
    }
}
