using System.Reflection;
using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramBot.Commands;
using TelegramBot.Services;

namespace TelegramBot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection LoadTelegramBot(this IServiceCollection collection, BotConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        Type baseType = typeof(ICommand);
        Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces().Contains(baseType))
            .ToList()
            .ForEach(t => collection.AddScoped(baseType, t));

        return collection
            .AddScoped<ITelegramBotClient>(x => new TelegramBotClient(configuration.ApiToken))
            .AddScoped<BotEngine>()
            .AddSingleton<BotContext>();
    }
}