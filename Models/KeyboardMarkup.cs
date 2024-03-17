namespace Models;

public class KeyboardMarkup
{
    private readonly int _maxButtonsInLine;

    public KeyboardMarkup(int maxButtonsInLine)
    {
        _maxButtonsInLine = maxButtonsInLine;
        Buttons = new List<IList<KeyboardButton>>();

        NewLine();
    }

    public KeyboardMarkup(IEnumerable<KeyboardButton> buttons, int maxButtonsInLine)
    {
        ArgumentNullException.ThrowIfNull(buttons);
        _maxButtonsInLine = maxButtonsInLine;
        Buttons = new List<IList<KeyboardButton>>();

        NewLine();
        buttons.ToList().ForEach(AddItem);
    }

    public IList<IList<KeyboardButton>> Buttons { get; }

    public KeyboardMarkup AddItems(params KeyboardButton[] buttons)
    {
        buttons.ToList().ForEach(AddItem);
        return this;
    }

    public KeyboardMarkup NewLine()
    {
        Buttons.Add(new List<KeyboardButton>());
        return this;
    }

    private void AddItem(KeyboardButton button)
    {
        if (Buttons.Last().Count >= _maxButtonsInLine)
        {
            NewLine();
        }

        Buttons.Last().Add(button);
    }
}