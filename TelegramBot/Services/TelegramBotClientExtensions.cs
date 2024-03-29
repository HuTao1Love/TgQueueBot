using Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using KeyboardButton = Models.KeyboardButton;

namespace TelegramBot.Services;

public static class TelegramBotClientExtensions
{
    public static InlineKeyboardMarkup ToTelegramKeyboardMarkup(this KeyboardMarkup fromMarkup)
    {
        ArgumentNullException.ThrowIfNull(fromMarkup);
        return new InlineKeyboardMarkup(fromMarkup.Buttons
            .Select(list => list.ToInlineKeyboardButtons()));
    }

    public static async Task<Message> EditTextAsync(
        this ITelegramBotClient client,
        long chatId,
        long messageId,
        string text,
        InlineKeyboardMarkup? markup,
        CancellationToken? token = null)
    {
        return await client.EditMessageTextAsync(
            new ChatId(chatId),
            (int)messageId,
            text: text,
            replyMarkup: markup,
            cancellationToken: token ?? CancellationToken.None);
    }

    public static async Task<Message> EditTextAsync(
        this Message message,
        ITelegramBotClient client,
        string text,
        InlineKeyboardMarkup? markup,
        CancellationToken? token = null)
    {
        ArgumentNullException.ThrowIfNull(message);
        return await client.EditTextAsync(message.Chat.Id, message.MessageId, text, markup, token);
    }

    public static async Task SafePinMessageAsync(
        this Message message,
        ITelegramBotClient client,
        bool disableNotification = false,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        try
        {
            await client.PinChatMessageAsync(message.Chat.Id, message.MessageId, disableNotification, token);
        }
        catch (ApiRequestException)
        {
        }
    }

    public static async Task SafeUnpinMessageAsync(
        this MessageIdentifier message,
        ITelegramBotClient client,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        try
        {
            await client.UnpinChatMessageAsync(message.ChatId, (int)message.MessageId, token);
        }
        catch (ApiRequestException)
        {
        }
    }

    public static async Task SafeUnpinMessageAsync(
        this Message message,
        ITelegramBotClient client,
        CancellationToken token = default)
        => await SafeUnpinMessageAsync(message.ToMessageIdentifier(), client, token);

    public static MessageIdentifier ToMessageIdentifier(this Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return new MessageIdentifier(message.Chat.Id, message.MessageId);
    }

    private static IEnumerable<InlineKeyboardButton> ToInlineKeyboardButtons(this IEnumerable<KeyboardButton> buttons)
    {
        return buttons.Select(button => InlineKeyboardButton.WithCallbackData(button.Text, button.Callback));
    }
}