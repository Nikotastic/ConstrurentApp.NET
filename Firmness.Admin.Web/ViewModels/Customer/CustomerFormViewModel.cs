using System.ComponentModel.DataAnnotations;

namespace Firmness.Web.ViewModels.Customer;

public class CustomerFormViewModel {
    public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "The name can only contain letters and spaces.")]
        [Display(Name = "Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The surname is mandatory.")]
        [StringLength(100, ErrorMessage = "The surname cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "The name can only contain letters and spaces.")]
        [Display(Name = "LastName")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "The email does not have a valid format.")]
        [StringLength(320, ErrorMessage = "The email is too long.")]

        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "The document is mandatory.")]
        [StringLength(50, ErrorMessage = "The document cannot exceed 50 characters.")]
        [Display(Name = "Document (NIT / ID Card)")]
        public string Document { get; set; } = string.Empty;

        [Phone(ErrorMessage = "The phone number is not in a valid format..")]
        [StringLength(50, ErrorMessage = "The phone is too long.")]
        [Display(Name = "Phone")]
        public string? Phone { get; set; }

        [StringLength(500, ErrorMessage = "The address cannot exceed 500 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "The name can only contain letters and spaces.")]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Asset")]
        public bool IsActive { get; set; } = true;

        //Optional file for customer's avatar/photo
        [Display(Name = "Photo (optional)")]
        public IFormFile? PhotoFile { get; set; }

        // If you store a url in entity, use this to show existing photo
        [Display(Name = "Photo (optional)")]
        [Url(ErrorMessage = "The URL is invalid..")]
        [StringLength(2000)]
        public string? PhotoUrl { get; set; }

    // Display property
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    public DateTime CreatedAt { get; set; }
}