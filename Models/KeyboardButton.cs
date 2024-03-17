namespace Models;

public abstract record KeyboardButton(string Text, string Callback)
{
    public record UserKeyboardButton : KeyboardButton
    {
        public UserKeyboardButton(int index, string emoji)
            : base(Text: $"{index + 1}{emoji}", Callback: $"{CallbackPrefix}{index}")
        {
        }

        public static string CallbackPrefix => "index-";
    }

    public sealed record ResetKeyboardButton() : KeyboardButton("Reset", ResetCallback)
    {
        public static string ResetCallback => "reset";
    }

    public sealed record StopKeyboardButton() : KeyboardButton("Stop", StopCallback)
    {
        public static string StopCallback => "stop";
    }

    public sealed record CancelKeyboardButton() : KeyboardButton("Cancel", CancelCallback)
    {
        public static string CancelCallback => "cancel";
    }
}