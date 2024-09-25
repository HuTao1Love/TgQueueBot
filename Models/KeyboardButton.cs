namespace Models;

public abstract record KeyboardButton(string Text, string Callback)
{
    public record UserKeyboardButton : KeyboardButton
    {
        public const string CallbackPrefix = "index-";

        public UserKeyboardButton(int index, string emoji)
            : base(Text: $"{index + 1}{emoji}", Callback: $"{CallbackPrefix}{index}")
        {
        }
    }

    public sealed record SkipMeKeyboardButton() : KeyboardButton("SkipMe", SkipMeCallback)
    {
        public const string SkipMeCallback = "skipme";
    }

    public sealed record ResetKeyboardButton() : KeyboardButton("Reset", ResetCallback)
    {
        public const string ResetCallback = "reset";
    }

    public sealed record StopKeyboardButton() : KeyboardButton("Stop", StopCallback)
    {
        public const string StopCallback = "stop";
    }

    public sealed record CancelKeyboardButton() : KeyboardButton("Cancel", CancelCallback)
    {
        public const string CancelCallback = "cancel";
    }
}