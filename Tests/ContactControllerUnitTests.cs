using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using ContactManager.Controllers;
using ContactManager.Models;
using ContactManager.Services;

namespace Tests
{
    public class ContactControllerTests
    {
        private readonly Mock<IContactService> _mockService;
        private readonly ContactController _controller;

        public ContactControllerTests()
        {
            // Set up the mock service and inject it into the controller
            _mockService = new Mock<IContactService>();
            _controller = new ContactController(_mockService.Object);
        }

        [Fact]
        public void Add_ModelStateInvalid_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Email is required");
            var contact = new Contact { FirstName = "Invalid" };

            // Act
            var result = _controller.Add(contact);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(s => s.Add(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public void Add_ValidModel_ReturnsOk()
        {
            var contact = new Contact { FirstName = "Valid" };
            // Tell the mock what to return when Add is called
            _mockService.Setup(s => s.Add(It.IsAny<Contact>())).Returns(contact);

            var result = _controller.Add(contact);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(contact, okResult.Value);
        }

        [Fact]
        public void Delete_ValidId_ReturnsOk()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.Delete(It.IsAny<Guid>())).Returns(true);

            var result = _controller.Delete(id);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Delete_InvalidId_ReturnsNotFound()
        {
            var id = Guid.NewGuid();
            // Simulate the service failing to find it
            _mockService.Setup(s => s.Delete(It.IsAny<Guid>())).Returns(false);

            var result = _controller.Delete(id);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetAll_ReturnsOkWithAllContacts()
        {
            var contacts = new List<Contact>
            {
                new Contact { FirstName = "Alice", Email = "alice@example.com" },
                new Contact { FirstName = "Bob",   Phone = "1234567" }
            };
            _mockService.Setup(s => s.GetAll()).Returns(contacts);

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(contacts, okResult.Value);
        }

        [Fact]
        public void GetAll_EmptyList_ReturnsOkEmptyList()
        {
            _mockService.Setup(s => s.GetAll()).Returns(new List<Contact>());

            var result = _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<Contact>>(okResult.Value);
            Assert.Empty(value);
        }

        [Fact]
        public void GetById_ExistingId_ReturnsOkContact()
        {
            var id = Guid.NewGuid();
            var contact = new Contact { Id = id, FirstName = "Alice", Email = "alice@example.com" };
            _mockService.Setup(s => s.GetById(id)).Returns(contact);

            var result = _controller.GetById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(contact, okResult.Value);
        }

        [Fact]
        public void GetById_MissingId_ReturnsOkNull()
        {
            var id = Guid.NewGuid();
            _mockService.Setup(s => s.GetById(id)).Returns((Contact?)null);

            var result = _controller.GetById(id);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Null(okResult.Value);
        }

        [Fact]
        public void Update_ModelStateInvalid_ReturnsBadRequest()
        {
            _controller.ModelState.AddModelError("FirstName", "First name is required.");
            var contact = new Contact { FirstName = "X", Email = "x@example.com" };

            var result = _controller.Update(contact);

            Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(s => s.Update(It.IsAny<Contact>()), Times.Never);
        }

        [Fact]
        public void Update_ExistingContact_ReturnsOk()
        {
            var contact = new Contact { FirstName = "Alice", Email = "alice@example.com" };
            _mockService.Setup(s => s.Update(It.IsAny<Contact>())).Returns(true);

            var result = _controller.Update(contact);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Update_MissingContact_ReturnsNotFound()
        {
            var contact = new Contact { FirstName = "Ghost", Email = "ghost@example.com" };
            _mockService.Setup(s => s.Update(It.IsAny<Contact>())).Returns(false);

            var result = _controller.Update(contact);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Search_ValidParameters_ReturnsOkResults()
        {
            var contacts = new List<Contact>
            {
                new Contact { FirstName = "Alice", Email = "alice@example.com" }
            };
            _mockService.Setup(s => s.Search("Alice", 0, 10)).Returns(contacts);

            var result = _controller.Search("Alice", 0, 10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(contacts, okResult.Value);
        }

        [Fact]
        public void Search_NegativePage_ReturnsBadRequest()
        {
            var result = _controller.Search("Alice", -1, 10);

            Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(s => s.Search(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Search_ZeroSize_ReturnsBadRequest()
        {
            var result = _controller.Search("Alice", 0, 0);

            Assert.IsType<BadRequestObjectResult>(result);
            _mockService.Verify(s => s.Search(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void Search_NullQuery_ReturnsOkResults()
        {
            var contacts = new List<Contact>();
            _mockService.Setup(s => s.Search(null, 0, 10)).Returns(contacts);

            var result = _controller.Search(null, 0, 10);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
