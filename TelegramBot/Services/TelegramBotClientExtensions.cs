using System.Collections;
using Models;
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

    private static IEnumerable<InlineKeyboardButton> ToInlineKeyboardButtons(this IEnumerable<KeyboardButton> buttons)
    {
        return buttons.Select(button => InlineKeyboardButton.WithCallbackData(button.Text, button.Callback));
    }
}