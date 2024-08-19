using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarketingContactManager.Models;
using MarketingContactManager.Contexts;

namespace MarketingContactManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ContactContext _context;
        private readonly ILogger<ContactController> _logger;

        public ContactController(ContactContext context, ILogger<ContactController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactModel>>> GetContacts()
        {
            return await _context.Contacts.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactModel>> GetContactModel(int id)
        {
            var contactModel = await _context.Contacts.FindAsync(id);

            if (contactModel == null)
            {
                return NotFound();
            }

            return contactModel;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactModel(int id, ContactModel contactModel)
        {
            if (string.IsNullOrEmpty(contactModel.Email) || string.IsNullOrEmpty(contactModel.FirstName) ||
    string.IsNullOrEmpty(contactModel.LastName) || string.IsNullOrEmpty(contactModel.PhoneNumber))
            {
                this._logger.LogInformation($"A required input is Null");
                return this.BadRequest();
            }

            // Validate emails before inserting
            if (!contactModel.Email.Contains('@'))
            {
                this._logger.LogInformation($"Email not formatted correctly for user email {contactModel.Email}");
                return this.BadRequest();
            }

            // Validate phone number
            if (contactModel.PhoneNumber.Length != 10)
            {
                this._logger.LogInformation($"Phone number length is not correct {contactModel.PhoneNumber}");
                return this.BadRequest();
            }

            contactModel.Id = id;

            _context.Entry(contactModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactModelExists(id))
                {
                    return NotFound($"Contact record with ID {id} not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<ContactModel>> PostContactModel(ContactModel contactModel)
        {
            if (string.IsNullOrEmpty(contactModel.Email) || string.IsNullOrEmpty(contactModel.FirstName) ||
                string.IsNullOrEmpty(contactModel.LastName) || string.IsNullOrEmpty(contactModel.PhoneNumber))
            {
                this._logger.LogInformation($"A required input is Null");
                return this.BadRequest();
            }

            // Validate emails before inserting
            if (!contactModel.Email.Contains('@'))
            {
                this._logger.LogInformation($"Email not formatted correctly for user email {contactModel.Email}");
                return this.BadRequest();
            }

            // Validate phone number
            if (contactModel.PhoneNumber.Length != 10)
            {
                this._logger.LogInformation($"Phone number length is not correct {contactModel.PhoneNumber}");
                return this.BadRequest();
            }

            _context.Contacts.Add(contactModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactModel", new { id = contactModel.Id }, contactModel);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactModel(int id)
        {
            var contactModel = await _context.Contacts.FindAsync(id);
            if (contactModel == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contactModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactModelExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}
