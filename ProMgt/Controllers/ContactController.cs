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

namespace ProMgt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ProjectDbContext _db;
        // private readonly ILogger _logger;
        // Http Context can help in recognizing the current user
        private readonly IdentityUserAccessor _userAccessor;

        //This is just an example. delete this after POC
        private readonly ApplicationDbContext _applicationDbContext;

        public ContactController(ProjectDbContext db, IdentityUserAccessor userAccessor, ApplicationDbContext applicationDbContext)
        {
            _db = db;
            _userAccessor = userAccessor;
            _applicationDbContext = applicationDbContext;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ContactResponse>> CreateContact(ContactResponse contact)
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

                var existingContact = await _applicationDbContext.Contacts
            .FirstOrDefaultAsync(c => c.UserId == user.Id && c.ContactUserId == contact.ContactUserId);

                if (existingContact != null)
                {
                    return Conflict("Contact already exists");
                }


                Contact _contact = new()
                {
                    UserId = user.Id,
                    ContactUserId = contact.ContactUserId,
                    CreatedAt = DateTime.UtcNow,

                };

                _applicationDbContext.Contacts.Add(_contact);
                
                await _applicationDbContext.SaveChangesAsync();

                return CreatedAtAction(nameof(GetContact), new { id = _contact.Id }, _contact);
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                // Log the error (uncomment ex variable name and write a log.)
                return Conflict("Record already exists.");
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
        public async Task<ActionResult<ContactResponse>> GetContact(int id)
        {
            var contactResponse = await _applicationDbContext.Contacts.FindAsync(id);

            if (contactResponse == null)
            {
                return NotFound();
            }

            ContactResponse _cr = new ContactResponse
            {
                Id = contactResponse.Id,
                ContactUserId = contactResponse.ContactUserId,
                CreatedAt = contactResponse.CreatedAt,
                UserId = contactResponse.UserId,
            };
            return Ok(_cr);
        }

        [Authorize]       
        [HttpGet("getallcontact")]
        public async Task<ActionResult<List<ContactDisplay>>> GetAllContacts()
        {
            var user = await _userAccessor.GetRequiredUserAsync(HttpContext);
            var response = await _applicationDbContext.Contacts
                .Where(c => c.UserId == user.Id)
                .Include(c => c.ContactUser)
                .ToListAsync();

            List<ContactDisplay> contacts = new();

            foreach (var contact in response) 
            {
                ContactDisplay cr = new ContactDisplay
                {
                   Id = contact.Id,
                   UserId = contact.UserId,
                   CreatedAt = contact.CreatedAt,
                   ContactUserId = contact.ContactUserId,
                   ContactFirstName = contact.ContactUser.FirstName,
                   ContactLastName = contact.ContactUser.LastName,
                   ContactEmail = contact.ContactUser.Email,
                };
                contacts.Add(cr);
            }

            return Ok(contacts);
        }

        [Authorize]
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteContact(int Id)
        {
            try
            {
                var contact = await _applicationDbContext.Contacts.FindAsync(Id);
                if (contact == null)
                {
                    return NotFound(new { message = "Project not found" });
                }

                _applicationDbContext.Contacts.Remove(contact);
                await _applicationDbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the contact" + ex.Message });
            }

        }
    }
}
