using System.Globalization;
using Application.Extensions;
using Contracts;
using Database.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Extensions;
using TelegramBot.Services;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var botConfig = new BotConfiguration();
config.Bind(botConfig);

var collection = new ServiceCollection();

collection
    .AddApplication()
    .LoadDatabase(botConfig)
    .LoadTelegramBot(botConfig)
    .AddScoped<BotConfiguration>(_ => botConfig)
    .AddScoped<CultureInfo>(_ => new CultureInfo("ru-Ru"));

ServiceProvider provider = collection.BuildServiceProvider();
using IServiceScope scope = provider.CreateScope();

BotEngine engine = scope.ServiceProvider.GetRequiredService<BotEngine>();

await engine.ListenForMessagesAsync();