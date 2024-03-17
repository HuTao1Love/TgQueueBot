using Contracts;
using Contracts.Repositories;
using Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection LoadDatabase(this IServiceCollection collection, BotConfiguration configuration)
        => collection
            .AddDbContextFactory<PostgresContext>(options => options.UseNpgsql(configuration.DbConnection))
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IQueueRepository, QueueRepository>()
            .AddScoped<IUserQueueRepository, UserQueueRepository>();
}