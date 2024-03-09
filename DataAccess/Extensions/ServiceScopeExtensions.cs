using Itmo.Dev.Platform.Postgres.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Extensions;

public static class ServiceScopeExtensions
{
    public static async void UseDatabase(this IServiceScope scope)
    {
        await scope.UsePlatformMigrationsAsync(default);
    }
}