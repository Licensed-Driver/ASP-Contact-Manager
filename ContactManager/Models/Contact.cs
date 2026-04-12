using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ContactManager.Models
{
    public enum ContactType {  Person, Organization }    // In case of business contacts
    public class Contact : IValidatableObject
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "First name can't be longer than 100 characters.")]
        public required string FirstName { get; set; }
        [StringLength(100, ErrorMessage = "Last name can't be longer than 100 characters.")]
        public string? LastName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? Phone { get; set; }

        // Make sure that at least one of email or phone number are provided
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Phone))
            {
                yield return new ValidationResult(
                    "You must provide either an Email address or a Phone number.",
                    new[] { nameof(Email), nameof(Phone) }  // Return which members the error is referring to
                    );
            }
        }

    }
}
