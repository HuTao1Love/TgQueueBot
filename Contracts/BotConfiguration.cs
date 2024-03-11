namespace Contracts;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class BotConfiguration
{
    public string ApiToken { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }
    public int BotCreator { get; set; }
    public int DefaultQueueSize { get; set; }
    public int MaxQueueNameLength { get; set; }
    public int MaxQueueSize { get; set; }
    public string BotPrefix { get; set; }
    public int MaxItemsPerKeyboardLine { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.