namespace Firmness.Core.Entities;

// class for users heritage from Person
public class User : Person
{
    // properties
    // Link to ASP.NET Identity user (FK scalar only).
    // Keep this as a simple string to avoid Core depending on Infrastructure.
    public string? IdentityUserId { get; set; }
    public string Username { get; set; } = String.Empty;
    public string PasswordHash { get; set; } = String.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginDate { get; set; }
    public DateTime? LastLogoutDate { get; set; }
    public DateTime? LastPasswordChangeDate { get; set; }

    
    // constructor 
    protected User() : base()
    {
    }

    // constructor with parameters
    public User(string firstName, string lastName, string username, string passwordHash, DateTime? dateOfBirth = null,
        string? email = null)
        : base(firstName, lastName, email)
    {
        Username = username;
        PasswordHash = passwordHash;
    }
    
    public override string FullName => $"{FirstName} {LastName} your username is {Username}".Trim();
}