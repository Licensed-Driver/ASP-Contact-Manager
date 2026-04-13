using Microsoft.AspNetCore.Mvc;
using Moq;
using ContactManager.Controllers;
using ContactManager.Models;
using ContactManager.Services;

namespace Tests
{
    public class DocumentationControllerUnitTests
    {
        private readonly Mock<IDocumentService> _mockService;
        private readonly DocumentationController _controller;

        public DocumentationControllerUnitTests()
        {
            _mockService = new Mock<IDocumentService>();
            _controller = new DocumentationController(_mockService.Object);
        }

        [Fact]
        public void Constructor_RegistersReadme_CallsAddDocument()
        {
            // Make sure the constructor called AddDocument with a path only once
            _mockService.Verify(s => s.AddDocument(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Get_DocumentNotFound_ReturnsNotFound()
        {
            // Setup GetByPath to return null for this test
            _mockService.Setup(s => s.GetByPath(It.IsAny<string>())).Returns((Document?)null);

            var result = await _controller.Get();

            Assert.IsType<NotFoundObjectResult>(result);
            _mockService.Verify(s => s.GetByPath(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Get_DocumentExists_ReturnsHtmlContentResult()
        {
            // Just make a temp md file to use
            var tmp = Path.GetTempFileName();
            var mdFile = Path.ChangeExtension(tmp, ".md");
            File.Move(tmp, mdFile);
            File.WriteAllText(mdFile, "# Hello World");
            // Make sure it returned the expected content converted value
            try
            {
                var document = new Document(mdFile);
                _mockService.Setup(s => s.GetByPath(It.IsAny<string>())).Returns(document);

                var result = await _controller.Get();

                var contentResult = Assert.IsType<ContentResult>(result);
                Assert.StartsWith("text/html", contentResult.ContentType);
                Assert.NotNull(contentResult.Content);
                Assert.Contains("<h1>", contentResult.Content);
            }
            finally { File.Delete(mdFile); }
        }

        [Fact]
        public async Task Get_DocumentExists_RendersMarkdownAsHtml()
        {
            var tmp = Path.GetTempFileName();
            var mdFile = Path.ChangeExtension(tmp, ".md");
            File.Move(tmp, mdFile);
            File.WriteAllText(mdFile, "**bold** and _italic_");
            // Make sure it translates markdown proper
            try
            {
                var document = new Document(mdFile);
                _mockService.Setup(s => s.GetByPath(It.IsAny<string>())).Returns(document);

                var result = await _controller.Get();

                var contentResult = Assert.IsType<ContentResult>(result);
                Assert.Contains("<strong>", contentResult.Content);
                Assert.Contains("<em>", contentResult.Content);
            }
            finally { File.Delete(mdFile); }
        }
    }
}
