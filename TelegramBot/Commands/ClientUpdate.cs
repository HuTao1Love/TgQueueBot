using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.Commands;

public class ClientUpdate : Update
{
    public ClientUpdate(Update from, ITelegramBotClient telegramBotClient)
    {
        TelegramBotClient = telegramBotClient;

        foreach (PropertyInfo property in typeof(Update).GetProperties())
        {
            if (property is { CanRead: true, CanWrite: true })
            {
                property.SetValue(this, property.GetValue(from));
            }
        }
    }

    public ITelegramBotClient TelegramBotClient { get; }

    public async Task AnswerText(string text, ParseMode? parseMode = null)
    {
        ArgumentNullException.ThrowIfNull(Message);
        await TelegramBotClient.SendTextMessageAsync(Message.Chat.Id, text, parseMode: parseMode);
    }

    public async Task AnswerWithKeyboard(string text, IReplyMarkup markup)
    {
        ArgumentNullException.ThrowIfNull(Message);
        ArgumentNullException.ThrowIfNull(markup);
        await TelegramBotClient.SendTextMessageAsync(Message.Chat.Id, text, replyMarkup: markup);
    }
}