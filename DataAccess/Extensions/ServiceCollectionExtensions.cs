using Contracts;
using Contracts.Repositories;
using Contracts.Services;
using DataAccess.Migrations;
using DataAccess.Repositories;
using Itmo.Dev.Platform.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection LoadDatabase(this IServiceCollection collection, IConfig config)
        => collection
            .AddPlatformPostgres(builder => builder.Configure(configuration =>
            {
                configuration.Host = config.Host;
                configuration.Port = config.Port;
                configuration.Username = config.Username;
                configuration.Password = config.Password;
                configuration.Database = config.Database;
                configuration.SslMode = "Prefer";
            }))
            .AddPlatformMigrations(typeof(CreateDatabase).Assembly)
            .AddScoped<IQueueRepository, QueueRepository>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IUserQueueRepository, UserQueueRepository>();
}