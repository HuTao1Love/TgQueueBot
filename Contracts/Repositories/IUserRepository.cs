using Models;

namespace Contracts.Repositories;

public interface IUserRepository
{
    Task<User> FindOrCreate(long tgId, string name);
    Task SetAdmin(long tgId, bool isAdmin);
}