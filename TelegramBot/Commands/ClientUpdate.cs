using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Exceptions;

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

    public async Task<Message> AnswerText(string text, ParseMode? parseMode = null)
    {
        if (Message is null) throw new MessageNotProvidedException();
        return await TelegramBotClient.SendTextMessageAsync(Message.Chat.Id, text, parseMode: parseMode);
    }

    public async Task<Message> AnswerWithKeyboard(string text, IReplyMarkup markup)
    {
        if (Message is null) throw new MessageNotProvidedException();
        ArgumentNullException.ThrowIfNull(markup);
        return await TelegramBotClient.SendTextMessageAsync(Message.Chat.Id, text, replyMarkup: markup);
    }

    public async Task AnswerCallbackQuery(string text)
    {
        if (CallbackQuery?.Message is null) throw new CallbackQueryNotProvidedException();

        await TelegramBotClient.AnswerCallbackQueryAsync(CallbackQuery.Id, text, true);
    }
}