using ContactManager.Models;
using ContactManager.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        // DI to connect service layer
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_contactService.GetAll());
        }

        [HttpGet("GetById/{id:guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            return Ok(_contactService.GetById(id));   // If it couldn't be added we return the null
        }

        [HttpPost("Add")]
        public IActionResult Add([FromBody] Contact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newContact = _contactService.Add(contact);
            if (newContact == null) return BadRequest("Could not create contact.");
            return Ok(newContact);
        }

        [HttpPut("Update")]
        public IActionResult Update([FromBody] Contact contact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return _contactService.Update(contact) ? Ok() : NotFound();
        }

        [HttpDelete("Delete/{id:guid}")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return _contactService.Delete(id) ? Ok() : NotFound();
        }

        [HttpGet("Search")]
        public IActionResult Search([FromQuery] string query, [FromQuery] int page, [FromQuery] int size)
        {
            if (page < 0 || size < 1) return BadRequest("Error: Page must be greater than 0 and size must be greater than 1.");
            return Ok(_contactService.Search(query, page, size));
        }
    }
}
