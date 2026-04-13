using Microsoft.AspNetCore.Mvc;
using Markdig;
using ContactManager.Services;

namespace ContactManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentationController : Controller
    {
        private IDocumentService _documentService;
        public DocumentationController(IDocumentService service) {
            _documentService = service;
            _documentService.AddDocument(Path.Combine(Directory.GetCurrentDirectory(), "..", "README.md"));
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            var document = _documentService.GetByPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "README.md"));
            if (document == null) { return NotFound("Could not find the README."); }
            return Content(Markdown.ToHtml((string)document.Content), "text/html", System.Text.Encoding.UTF8);
        }
    }
}
