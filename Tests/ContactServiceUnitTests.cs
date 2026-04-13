using ContactManager.Services;
using ContactManager.Models;

namespace Tests
{
    public class ContactServiceUnitTests
    {
        [Fact]
        public void Validate_GetAllWithoutContacts_ReturnsEmpty()
        {
            // Arrange
            var service = new ContactService();

            // Act
            IEnumerable<Contact> result = service.GetAll();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Validate_AddWithValidContact_IsValid()
        {
            var service = new ContactService();
            var contact = new Contact
            {
                FirstName = "Test",
                LastName = "User",
                Email = "testing@user.com"
            };

            var result = service.Add(contact);
            
            Assert.NotNull(result);
            Assert.Equal(contact.FirstName, result.FirstName);
            Assert.Equal(contact.LastName, result.LastName);
            Assert.Equal(contact.Email, result.Email);
            Assert.NotEmpty(result.Id.ToString());
        }

        [Fact]
        public void Validate_GetById_ReturnsContact()
        {
            var service = new ContactService();
            var contact = service.Add(new Contact
            {
                FirstName = "Test",
                LastName = "User",
                Email = "testing@user.com"
            });

            var result = service.GetById(contact.Id);

            Assert.NotNull(result);
            Assert.Equal(contact.Id, result.Id);
        }

        [Fact]
        public void Validate_GetByIdWithoutId_ReturnsNull()
        {
            var service = new ContactService();

            var result = service.GetById(Guid.Empty);

            Assert.Null(result);
        }

        [Fact]
        public void Validate_GetAllWithContacts_ReturnsAll()
        {
            var service = new ContactService();
            var contact = service.Add(new Contact
            {
                FirstName = "Test2",
                LastName = "User2",
                Email = "testing2@user.com"
            });

            var result = service.GetAll();

            Assert.Single(result);
            Assert.Contains(result, c => c.Id == contact.Id);
        }

        [Fact]
        public void Validate_UpdateWithInvalidContact_ReturnsFalse()
        {
            var service = new ContactService();
            var contact = new Contact
            {
                Id = Guid.NewGuid(), // This ID isn't in the service, so Update should fail
                FirstName = "Super",
                LastName = "Tester",
                Email = "super@tester.com"
            };

            var result = service.Update(contact);

            Assert.False(result);
        }

        [Fact]
        public void Validate_UpdateWithValidContact_Updates()
        {
            var service = new ContactService();
            var originalContact = service.Add(new Contact
            {
                FirstName = "OldName",
                LastName = "OldLastName",
                Email = "old@email.com",
                Phone = "1111111111"
            });

            var updatedContact = new Contact
            {
                Id = originalContact.Id,
                FirstName = "NewName",
                LastName = "NewLastName",
                Email = "new@email.com",
                Phone = "4033333333"
            };

            var result = service.Update(updatedContact);
            var retrieved = service.GetById(originalContact.Id);

            Assert.True(result);
            Assert.NotNull(retrieved);
            Assert.Equal(updatedContact.FirstName, retrieved.FirstName);
            Assert.Equal(updatedContact.LastName, retrieved.LastName);
            Assert.Equal(updatedContact.Email, retrieved.Email);
            Assert.Equal(updatedContact.Phone, retrieved.Phone);
        }

        [Fact]
        public void Validate_DeleteWithInvalidId_ReturnsFalse()
        {
            var service = new ContactService();

            var result = service.Delete(Guid.Empty);

            Assert.False(result);
        }

        [Fact]
        public void Validate_DeleteWithValidId_Deletes()
        {
            var service = new ContactService();
            var contact = service.Add(new Contact { FirstName = "ToDelete" });

            var result = service.Delete(contact.Id);
            var deleted = service.GetById(contact.Id);

            Assert.True(result);
            Assert.Null(deleted);
        }

        [Fact]
        public void Search_WithValidQuery_OrdersByBestMatch()
        {
            var service = new ContactService();
            service.Add(new Contact { FirstName = "Zachary", Email = "zach@example.com" });
            service.Add(new Contact { FirstName = "Brittany", Email = "brittany@example.com" });
            service.Add(new Contact { FirstName = "Brian", Email = "brian@example.com" });

            var results = service.Search("Britt", 0, 10).ToList();

            Assert.Equal(3, results.Count);
            Assert.Equal("Brittany", results.First().FirstName);
        }

        [Fact]
        public void Search_WithPagination_ReturnsCorrectSlice()
        {
            var service = new ContactService();
            for (int i = 0; i < 5; i++)
            {
                service.Add(new Contact { FirstName = $"User{Math.Pow(10, i)}" });
            }

            var page0 = service.Search("User", 0, 2).ToList();
            var page1 = service.Search("User", 1, 2).ToList();

            Assert.Equal(2, page0.Count);
            Assert.Equal(2, page1.Count);
            Assert.NotEqual(page0.First().Id, page1.First().Id);
        }
    }
}
