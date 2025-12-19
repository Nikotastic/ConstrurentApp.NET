using Firmness.Domain.Entities;

namespace Firmness.Application.Interfaces;

//Interface for user account services
public interface IUserAccountService
{
    Task<(string UserId, string Token)> CreateUserAndGenerateActivationTokenAsync(string email, string firstName, string lastName);
}
