namespace Firmness.Core.Entities;

public class Customer : Person
{
    // identity user id for the customer
    public string? IdentityUserId { get; set; }
    
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    
    public Customer() { }
    
    // constructor with parameters
    public Customer(string firstName, string lastName, string email) : base(firstName, lastName, email)
    {
    }
    
    public override string FullName => $"{FirstName} {LastName}".Trim();
}