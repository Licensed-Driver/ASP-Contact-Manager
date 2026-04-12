using System.ComponentModel.DataAnnotations;

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
        // Found out during testing that EmailAddress uses the RFC for validation which doesn't enforce common-sense rules (like missing TLDs, iinvalid chars like :\"
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format.")]
        public string? Email {
            get => _email;
            set => _email = string.IsNullOrWhiteSpace(value) ? null : value;    // This for email and phone since empty data from frontend forms would throw an error
        }
        [Phone(ErrorMessage = "Invalid phone number format.")]
        // So that we don't let through infinite length strings or tiny strings
        [StringLength(20, MinimumLength = 7, ErrorMessage = "Phone number must be between 7 and 15 numbers.")]  // Technically the smallest possible phone number is 7, and longest is 15 but also allowing some characters
        public string? Phone {
            get => _phone;
            set => _phone = string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private string? _phone;
        private string? _email;

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
