using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;
using ContactManager.Models;

namespace Tests
{
    public class ContactModelUnitTests
    {
        [Fact]
        public void Validate_NoEmailOrPhone_ReturnsValidationError()
        {
            var contact = new Contact
            {
                FirstName = "Test",
                LastName = "User",
                Email = "",
                Phone = ""
            };
            var context = new ValidationContext(contact);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contact, context, results, true);

            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("You must provide either an Email address or a Phone number."));    // Contains since it's technically a list and we gotta iterate through it
        }

        [Fact]
        public void Validate_NoFirstName_ReturnsValidationError() {
            var contact = new Contact
            {
                FirstName = "",
                LastName = "User",
                Email = "testing@user.com",
                Phone = ""
            };
            var context = new ValidationContext(contact);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contact, context, results, true);

            Assert.False(isValid);
        }

        [Fact]
        public void Validate_NoLastName_IsValid()
        {
            var contact = new Contact
            {
                FirstName = "Test",
                LastName = "",
                Email = "user@bingus.com",
                Phone = ""
            };
            var context = new ValidationContext(contact);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contact, context, results, true);

            Assert.True(isValid);
        }

        [Fact]
        public void Validate_HasEmailButNoPhone_IsValid()
        {
            var contact = new Contact
            {
                FirstName = "Test",
                LastName = "User",
                Email = "user@testing.com",
                Phone = ""
            };
            var context = new ValidationContext(contact);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);

            results.Clear();

            contact.Phone = null;
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);
        }

        [Fact]
        public void Validate_HasPhoneButNoEmail_IsValid()
        {
            var contact = new Contact
            {
                FirstName = "Test",
                LastName = "User",
                Email = "",
                Phone = "1234567891"
            };
            var context = new ValidationContext(contact);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);

            results.Clear();

            contact.Email = null;
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);
        }

        [Fact]
        public void Validate_HasPhoneAndEmail_IsValid()
        {
            var contact = new Contact
            {
                FirstName = "Test",
                LastName = "User",
                Email = "testing@user.com",
                Phone = "1234567891"
            };
            var context = new ValidationContext(contact);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contact, context, results, true);

            Assert.True(isValid);
        }

        [Fact]
        public void Validate_IncorrectEmailFormat_ReturnsValidationError()
        {
            var contact = new Contact
            {
                FirstName = "Test",
                LastName = "User",
                Email = "@testing.com",
                Phone = null
            };
            var context = new ValidationContext(contact);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);

            results.Clear();

            contact.Email = "user";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);

            results.Clear();

            contact.Email = ":)@=//.com";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);

            results.Clear();

            contact.Email = "user.com";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);

            results.Clear();

            contact.Email = "@.com";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);

            results.Clear();

            contact.Email = "user@";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);

            results.Clear();

            contact.Email = "user@testing";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);

            results.Clear();

            contact.Email = "user@.com";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);
        }

        [Fact]
        public void Validate_IncorrectPhoneFormat_ReturnsValidationError()
        {
            var contact = new Contact
            {
                FirstName = "Test",
                LastName = "User",
                Email = "",
                Phone = "()-"
            };
            var context = new ValidationContext(contact);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);

            results.Clear();

            contact.Phone = "characters";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);

            results.Clear();

            contact.Phone = "1234567890123455678556";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);

            results.Clear();

            contact.Phone = "321312";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.False(isValid);
        }

        [Fact]
        public void Validate_CorrectPhoneFormat_IsValid()
        {
            var contact = new Contact
            {
                FirstName = "Test",
                LastName = "User",
                Email = null,
                Phone = "(403) 796-3080"
            };
            var context = new ValidationContext(contact);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);

            results.Clear();

            contact.Phone = "4037333080";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);

            results.Clear();

            contact.Phone = "403-733-3080";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);

            results.Clear();

            contact.Phone = "(403)733-3080";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);

            results.Clear();

            contact.Phone = "403 733-3080";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);

            results.Clear();

            contact.Phone = "403 733 3080";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);

            results.Clear();

            contact.Phone = "+14037333080";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);

            results.Clear();

            contact.Phone = "+1 4037963080";
            isValid = Validator.TryValidateObject(contact, context, results, true);
            Assert.True(isValid);
        }
    }
}