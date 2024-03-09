namespace Models;

public class KeyboardMarkup
{
    private readonly int _maxButtonsInLine;

    public KeyboardMarkup(IEnumerable<KeyboardButton> buttons, int maxButtonsInLine)
    {
        ArgumentNullException.ThrowIfNull(buttons);
        _maxButtonsInLine = maxButtonsInLine;
        Buttons = new List<IList<KeyboardButton>>();
        NewLine();

        buttons.ToList().ForEach(i => AddItem(i));
    }

    public IList<IList<KeyboardButton>> Buttons { get; }

    public KeyboardMarkup AddItem(KeyboardButton button)
    {
        if (Buttons.Last().Count >= _maxButtonsInLine)
        {
            NewLine();
        }

        Buttons.Last().Add(button);

        return this;
    }

    public KeyboardMarkup AddItems(params KeyboardButton[] buttons)
    {
        buttons.ToList().ForEach(i => AddItem(i));
        return this;
    }

    public KeyboardMarkup NewLine()
    {
        Buttons.Add(new List<KeyboardButton>());
        return this;
    }
}