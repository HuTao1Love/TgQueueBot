namespace TelegramBot.Rules;

public abstract class MainRuleAttribute : Attribute
{
    public virtual void Initialize(IServiceProvider provider)
    {
    }

    public abstract Task<bool> Check(ClientUpdate update, CancellationToken token);
}