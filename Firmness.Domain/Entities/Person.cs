using System.ComponentModel.DataAnnotations.Schema;

namespace Firmness.Domain.Entities;

// abstract class for all persons
public abstract class Person : BaseEntity
{
    [NotMapped]
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public string Email { get; set; }
    
    // constructor 
    protected Person() { }
    
    // constructor with parameters
    protected Person(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
    
    // property for full name
    public virtual string FullName => $"{FirstName} {LastName}";
    
}