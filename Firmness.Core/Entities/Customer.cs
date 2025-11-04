namespace Firmness.Core.Entities;

public class Customer : Person
{
    // identity user id for the customer
    public string? IdentityUserId { get; set; }
    
    public string Document { get; set; } = string.Empty; 
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public string? PhotoFile { get; set; }
    
    public string? PhotoUrl { get; set; }
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    protected Customer() { }

    public Customer(string firstName, string lastName, string email) : base(firstName, lastName, email)
    {
    }

    public override string FullName => $"{FirstName} {LastName}".Trim();
}