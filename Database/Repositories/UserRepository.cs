using Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Database.Repositories;

public class UserRepository(IDbContextFactory<PostgresContext> contextFactory) : IUserRepository
{
    public async Task<User> FindOrCreate(long tgId, string name)
    {
        await using PostgresContext context = await contextFactory.CreateDbContextAsync();
        UserData? data = await context.Users.FirstOrDefaultAsync(i => i.TgId == tgId);

        if (data is null)
        {
            data = new UserData()
            {
                TgId = tgId,
                Name = name,
                IsAdmin = false,
            };

            await context.Users.AddAsync(data);
        }
        else
        {
            data.Name = name;
            context.Users.Update(data);
        }

        await context.SaveChangesAsync();

        return new User(data.Id, data.TgId, data.Name, data.IsAdmin);
    }

    public async Task SetAdmin(long tgId, bool isAdmin)
    {
        await using PostgresContext context = await contextFactory.CreateDbContextAsync();
        UserData? data = await context.Users.FirstOrDefaultAsync(i => i.TgId == tgId);

        if (data is null) return;

        data.IsAdmin = isAdmin;
        context.Users.Update(data);

        await context.SaveChangesAsync();
    }
}