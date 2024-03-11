using Contracts;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Commands;

namespace TelegramBot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection LoadTelegramBot(this IServiceCollection collection, BotConfiguration configuration)
    {
        typeof(CommandBase).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(typeof(CommandBase)))
            .ToList()
            .ForEach(t => collection.AddScoped(typeof(CommandBase), t));

        return collection.AddScoped<BotEngine>();
    }
}