using Firmness.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Firmness.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    
    public virtual Firmness.Domain.Entities.Person? Person { get; set; }
}