namespace TelegramBot.Rules;

public abstract class RuleAttribute : Attribute
{
    public virtual void Initialize(IServiceProvider provider)
    {
    }

    public abstract Task<bool> Check(ClientUpdate update, CancellationToken token);
}