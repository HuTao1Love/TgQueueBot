using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramBot.Commands;

namespace TelegramBot.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection LoadTelegramBot(this IServiceCollection collection, BotConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        Type commandBaseType = typeof(CommandBase);
        commandBaseType.Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.IsSubclassOf(commandBaseType))
            .ToList()
            .ForEach(t => collection.AddScoped(commandBaseType, t));

        collection.AddScoped<ITelegramBotClient>(x => new TelegramBotClient(configuration.ApiToken));

        return collection.AddScoped<BotEngine>();
    }
}