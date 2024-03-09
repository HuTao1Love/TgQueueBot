using Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Environment.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection LoadEnvironment(this IServiceCollection collection)
        => collection.AddScoped<IConfig, Config>();
}