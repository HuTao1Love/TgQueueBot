using Contracts.Services;
using Models;

namespace Contracts.Repositories;

public interface IUserRepository
{
    Task<User> FindOrCreate(long tgId, string name, bool isAdmin);
}